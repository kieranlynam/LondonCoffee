﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using CoffeeClientPrototype.Model;
using CoffeeClientPrototype.ViewModel.Details;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ViewModel.Tests.Details
{
    [TestClass]
    public class DetailsViewModelTests
    {
        [TestMethod]
        public async Task PropertiesPopulatedWhenNavigatedTo()
        {
            using (var context = new Context())
            {
                context.Cafes.Add(
                    new Cafe
                    {
                        Id = 1,
                        Name = "Cafe A",
                        Address = "1 Main Street",
                        PostCode = "A1 11A",
                        Latitude = 15,
                        Longitude = -10,
                        CoffeeRating = 3.5,
                        AtmosphereRating = 4.75,
                        NumberOfVotes = 200
                    });

                await context.ViewModel.OnNavigatedTo(
                    new Dictionary<string, object>
                    {
                        { "Id", 1 }
                    });
                
                Assert.AreEqual("Cafe A", context.ViewModel.Name, "Name");
                Assert.AreEqual("1 Main Street", context.ViewModel.Address, "Address");
                Assert.AreEqual("A1 11A", context.ViewModel.PostCode, "PostCode");
                Assert.AreEqual(15, context.ViewModel.Latitude, "Latitude");
                Assert.AreEqual(-10, context.ViewModel.Longitude, "Longitude");
                Assert.AreEqual(3.5, context.ViewModel.CoffeeRating, "CoffeeRating");
                Assert.AreEqual(4.75, context.ViewModel.AtmosphereRating, "AtmosphereRating");
                Assert.AreEqual(200, context.ViewModel.NumberOfVotes, "NumberOfVotes");
            }
        }

        [TestMethod]
        public async Task ReviewsPopulatedWhenNavigatedTo()
        {
            using (var context = new Context())
            {
                var cafe = new Cafe { Id = 1 };
                context.Cafes.Add(cafe);
                context.Reviews[cafe] = new[]
                    {
                        new Review { Comment = "Good!" },
                        new Review { Comment = "Bad!" },
                        new Review { Comment = "Ugly!" }
                    };

                await context.ViewModel.OnNavigatedTo(
                    new Dictionary<string, object>
                    {
                        { "Id", cafe.Id }
                    });

                CollectionAssert.AreEquivalent(
                    context.Reviews[cafe].ToArray(),
                    context.ViewModel.Reviews);
            }
        }

        [TestMethod]
        public async Task ReviewsSortedNewestToOldest()
        {
            using (var context = new Context())
            {
                var cafe = new Cafe { Id = 1 };
                context.Cafes.Add(cafe);
                context.Reviews[cafe] = new[]
                    {
                        new Review
                        {
                            Comment = "Yesterday",
                            CreatedDate = DateTime.Today.AddDays(-1)
                        },
                        new Review
                        {
                            Comment = "Ancient!",
                            CreatedDate = DateTime.Today.AddYears(-5)
                        },
                        new Review
                        {
                            Comment = "Today",
                            CreatedDate = DateTime.Today
                        }
                    };

                await context.ViewModel.OnNavigatedTo(
                    new Dictionary<string, object>
                    {
                        { "Id", cafe.Id }
                    });

                Assert.AreEqual("Today", context.ViewModel.Reviews[0].Comment);
                Assert.AreEqual("Yesterday", context.ViewModel.Reviews[1].Comment);
                Assert.AreEqual("Ancient!", context.ViewModel.Reviews[2].Comment);
            }
        }

        [TestMethod]
        public async Task SubmitUserReview()
        {
            using (var context = new Context())
            {
                var cafe = new Cafe { Id = 1 };
                context.Cafes.Add(cafe);

                await context.ViewModel.OnNavigatedTo(
                    new Dictionary<string, object>
                    {
                        { "Id", cafe.Id }
                    });

                context.ViewModel.UserReview.Comment = "New!";
                context.ViewModel.UserReview.Submit.Execute(null);

                Assert.IsTrue(context.Reviews.ContainsKey(cafe),
                    "Expected a comment to be submitted");
                var reviews = context.Reviews[cafe];
                Assert.AreEqual(1, reviews.Count);
                Assert.AreEqual("New!", reviews.Last().Comment, "Review text");
            }
        }

        [TestMethod]
        public async Task CannotSubmitReviewWithoutText()
        {
            using (var context = new Context())
            {
                var cafe = new Cafe { Id = 1 };
                context.Cafes.Add(cafe);

                await context.ViewModel.OnNavigatedTo(
                    new Dictionary<string, object>
                    {
                        { "Id", cafe.Id }
                    });

                context.ViewModel.UserReview.Comment = "";
                Assert.IsFalse(context.ViewModel.UserReview.Submit.CanExecute(null));

                context.ViewModel.UserReview.Comment = "Something";
                Assert.IsTrue(context.ViewModel.UserReview.Submit.CanExecute(null));
            }
        }

        [TestMethod]
        public async Task CannotSubmitReviewBeforeNavigating()
        {
            using (var context = new Context())
            {
                var cafe = new Cafe { Id = 1 };
                context.Cafes.Add(cafe);

                context.ViewModel.UserReview.Comment = "Something";
                Assert.IsFalse(context.ViewModel.UserReview.Submit.CanExecute(null));

                await context.ViewModel.OnNavigatedTo(
                    new Dictionary<string, object>
                    {
                        { "Id", cafe.Id }
                    });

                Assert.IsTrue(context.ViewModel.UserReview.Submit.CanExecute(null));
            }
        }

        private class Context : BaseTestContext
        {
            public DetailsViewModel ViewModel { get; private set; }

            public Dictionary<Cafe, IList<Review>> Reviews { get { return this.DataService.Reviews; } }

            public Context()
            {
                this.ViewModel = new DetailsViewModel(
                    this.DataService,
                    new MockNavigationService());
            }
        }
    }
}
