﻿using System;
using System.Collections.Generic;
using System.Linq;
using Backend.Entities;
using Backend.Services;
using Xunit;

namespace UnitTestProject
{
    public class LeaveCalculationsTests
    {
        private LeaveService _leaveService;
        private ServicesFixture _servicesFixture;

        public LeaveCalculationsTests()
        {
            _servicesFixture = new ServicesFixture();
            _servicesFixture.SetupData();
            _leaveService = _servicesFixture.Get<LeaveService>();
        }


        public static IEnumerable<object[]> LeaveMemberData()
        {
            IEnumerable<(DateTime, DateTime, int)> MakeValues()
            {
                //May 1st 2015 is a Friday
                yield return (new DateTime(2015, 5, 1), new DateTime(2015, 5, 1), 1);
                //ensure time gets truncated
                yield return (new DateTime(2015, 5, 1), new DateTime(2015, 5, 1, 23, 59, 59), 1);
                //the second is a satruday, so it's not counted
                yield return (new DateTime(2015, 5, 1), new DateTime(2015, 5, 2), 1);
                //this spans the weekend, so we subtract 2 days
                yield return (new DateTime(2015, 5, 1), new DateTime(2015, 5, 7), 5);
                //this doesn't span a weekend
                yield return (new DateTime(2015, 5, 4), new DateTime(2015, 5, 5), 2);
                //gone for the whole week
                yield return (new DateTime(2015, 5, 4), new DateTime(2015, 5, 8), 5);
                //gone for 2 weeks
                yield return (new DateTime(2015, 5, 4), new DateTime(2015, 5, 15), 10);
                //gone for 2 weeks, but start date is a saturday, and end date is sunday
                yield return (new DateTime(2015, 5, 2), new DateTime(2015, 5, 17), 10);
            }

            return MakeValues().Select(tuple => tuple.ToArray());
        }

        [Theory]
        [MemberData(nameof(LeaveMemberData))]
        public void ShouldMatchExpectedLeave(DateTime startDate, DateTime endDate, int expectedDays)
        {
            var result = LeaveService.TotalLeaveUsed(new[] {new LeaveRequest(startDate, endDate)});
            Assert.Equal(expectedDays, result);
        }

        public static IEnumerable<object[]> RolesMemberData()
        {
            IEnumerable<(int?, IList<PersonRole> )> MakeValues()
            {
                var twoYearsAgo = DateTime.Now - TimeSpan.FromDays(2 * 366);
                yield return (10,
                    new List<PersonRole>
                    {
                        new PersonRole {Active = true, IsStaffPosition = true, StartDate = twoYearsAgo}
                    });
                yield return (10,
                    new List<PersonRole>
                    {
                        new PersonRole {Active = true, IsStaffPosition = true, StartDate = twoYearsAgo},
                        //director position doesn't count to the 20 days because it's not active
                        new PersonRole
                        {
                            IsDirectorPosition = true,
                            StartDate = new DateTime(2000, 1, 1),
                            EndDate = new DateTime(2002, 1, 1)
                        },
                    });
                yield return (20, new List<PersonRole>
                {
                    new PersonRole {Active = true, IsDirectorPosition = true, StartDate = twoYearsAgo}
                });
                yield return (15, new List<PersonRole>
                {
                    new PersonRole {Active = true, IsStaffPosition = true, StartDate = twoYearsAgo},
                    new PersonRole
                    {
                        IsStaffPosition = true,
                        StartDate = new DateTime(2000, 1, 1),
                        EndDate = new DateTime(2002, 1, 1)
                    },
                    new PersonRole
                    {
                        IsStaffPosition = true,
                        StartDate = new DateTime(2002, 1, 2),
                        EndDate = new DateTime(2013, 1, 1)
                    }
                });
                yield return (20, new List<PersonRole>
                {
                    new PersonRole {Active = true, IsStaffPosition = true, StartDate = twoYearsAgo},
                    new PersonRole
                    {
                        IsStaffPosition = true,
                        StartDate = new DateTime(2000, 1, 1),
                        EndDate = new DateTime(2002, 1, 1)
                    },
                    new PersonRole
                    {
                        IsStaffPosition = true,
                        StartDate = new DateTime(2002, 1, 2),
                        EndDate = new DateTime(2013, 1, 1)
                    },
                    new PersonRole
                    {
                        IsStaffPosition = true,
                        StartDate = new DateTime(1995, 1, 1),
                        EndDate = new DateTime(2000, 1, 1)
                    }
                });
                yield return (null, new List<PersonRole>());
                yield return (null, new List<PersonRole>
                {
                    new PersonRole {Active = true, StartDate = twoYearsAgo},
                });
            }

            return MakeValues().Select(tuple => tuple.ToArray());
        }

        [Theory]
        [MemberData(nameof(RolesMemberData))]
        public void ShouldCalculateLeaveAllowed(int? expected, IList<PersonRole> personRoles)
        {
            var result = LeaveService.LeaveAllowed(LeaveType.Vacation, personRoles);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ListAllLeaveWorks()
        {
            var personAndLeaveDetailses = _leaveService.PeopleWithLeave(null);
            Assert.NotEmpty(personAndLeaveDetailses);
        }
    }
}