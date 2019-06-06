﻿using Api.Controllers;
using Entities.Entities;
using Entities.Responses;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using Services.Interfaces.Features.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Tests.Api.Controllers {

    public class ApiControllerTests {
        private readonly Mock<IBookSearchService> _bookSearchServiceMock;
        private readonly ApiController _controller;

        public ApiControllerTests() {
            _bookSearchServiceMock = new Mock<IBookSearchService>();
            _controller = new ApiController(_bookSearchServiceMock.Object);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Books_ShouldReturnMessageIfNoKeywordIsGiven(string data) {
            var expected = new JsonResult("no keyword given");
            var actual = _controller.Books(data);
            Assert.Equal(expected.Value.ToString(), actual.Value.ToString());
        }

        [Fact]
        public void Books_ShouldReturnData() {
            var data = new[]{
                new Book {
                    Description = "description",
                    ImageUrl = "imageUrl",
                    PublicationDate = new DateTime(2018, 6, 3),
                    SmallImageUrl = "smallImageUrl"
                }
            };
            _bookSearchServiceMock.Setup(mock => mock.Search(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>())).Returns(data);
            var expected = JsonConvert.SerializeObject(data.Select(item => new BookResponse(item)));
            var actual = JsonConvert.SerializeObject((IEnumerable<BookResponse>)_controller.Books("some words here").Value);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Books_ShouldReturnDataWhenSearchResponseItIsEmpty() {
            _bookSearchServiceMock.Setup(mock => mock.Search(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>())).Returns(new Book[0]);
            var expected = JsonConvert.SerializeObject(new BookResponse[0]);
            var actual = JsonConvert.SerializeObject((IEnumerable<BookResponse>)_controller.Books("some words here").Value);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Book_should_call_service() {
            _bookSearchServiceMock.Setup(mock => mock.FindById(It.IsAny<int>())).Returns(new Book());
            _controller.Book(10);

            _bookSearchServiceMock.Verify(mock => mock.FindById(10));
            _bookSearchServiceMock.Verify(mock => mock.FindById(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public void Book_should_return_book() {
            var book = new Book {
                Id = 10,
                Title = "some title",
                Description = "some description",
            };
            _bookSearchServiceMock.Setup(mock => mock.FindById(It.IsAny<int>())).Returns(book);
            var expected = JsonConvert.SerializeObject(new BookResponse(book));
            var actual = JsonConvert.SerializeObject((BookResponse)_controller.Book(10).Value);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Book_should_return_no_results_when_record_not_found() {
            _bookSearchServiceMock.Setup(mock => mock.FindById(It.IsAny<int>())).Returns((Book)null);
            var expected = JsonConvert.SerializeObject(null);
            var actual = JsonConvert.SerializeObject((BookResponse)_controller.Book(10).Value);
            Assert.Equal(expected, actual);
        }
    }
}