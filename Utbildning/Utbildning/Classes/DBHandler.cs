﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Web;
using Utbildning.Models;

namespace Utbildning.Classes
{
    public static class DBHandler
    {
        public static int GetBookings(this CourseOccasion co)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                List<Booking> bookings = db.Bookings.ToList();
                return bookings.Where(x => x.CourseOccasionId == co.Id).Sum(y => y.Bookings);
            }
        }

        public static int GetAvailableBookings(this CourseOccasion co) => co.MaxPeople - co.GetBookings();

        public static bool EnoughAvailable(this CourseOccasion co, int NewBookings) => co.GetAvailableBookings() - NewBookings >= 0;

        public static bool EnoughBookings(this CourseOccasion co) => co.GetBookings() >= co.MinPeople;

        public static string Format(this DateTime dt)
        {
            string dtString = dt.ToString("dddd, dd MMMM yyyy", new CultureInfo("sv-SE"));
            if (dtString.Length > 0)
                return char.ToUpper(dtString[0]) + dtString.Substring(1);
            return "Failed to load.";
        }

        public static Course GetCourse(this CourseOccasion co)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                return db.Courses.ToList().Where(x => x.Id == co.CourseId).First();
            }
        }


        public static Course GetCourse(int CourseId)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                return db.Courses.ToList().Where(x => x.Id == CourseId).First();
            }
        }

        public static CourseOccasion GetCourseOccasion(this Booking booking)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                return db.CourseOccasions.Find(booking.CourseOccasionId);
            }
        }

        public static CourseOccasion GetCourseOccasion(int CourseOccasionId)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                return db.CourseOccasions.Find(CourseOccasionId);
            }
        }

        public static bool ValidUser(this IPrincipal User, Course course) => User.Identity.Name == course.Email;

        public static bool ValidUser(this IPrincipal User, CourseOccasion courseOccasion) => User.Identity.Name == courseOccasion.GetCourse().Email;

        public static bool ValidUser(this IPrincipal User, Booking booking) => User.Identity.Name == booking.GetCourseOccasion().GetCourse().Email;
    }
}