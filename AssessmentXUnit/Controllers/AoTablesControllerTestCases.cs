﻿using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngularAssessmentAPI.Services.Interface;
using AngularAssessmentAPI.Controllers;
using AngularAssessmentAPI.Models;
using Microsoft.AspNetCore.Http;
using Azure;

namespace AssessmentXUnit.Controllers
{
    public class AoTablesControllerTestCases
    {
        private readonly IFixture fixture;
        public readonly Mock<ITableInterface> tableInterface;
        public readonly AoTablesController tableController;

        public AoTablesControllerTestCases()
        {
            fixture = new Fixture();
            tableInterface = fixture.Freeze<Mock<ITableInterface>>();
            tableController = new AoTablesController(tableInterface.Object);
        }

        //TestCases for AoTable
        [Fact]
        public async Task GetAllTables_ReturnsOkResultWithValidDatas()
        {
            // Arrange
            var mockTableInterface = new Mock<ITableInterface>();
            mockTableInterface.Setup(repo => repo.GetAllTables()).ReturnsAsync(new List<AoTable> { new AoTable { Id = Guid.NewGuid(), Name = "Table1" } });
            var controller = new AoTablesController(mockTableInterface.Object);


            // Act
            var result = await controller.GetAllTables() as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.NotNull(result.Value);
            var tables = Assert.IsType<List<AoTable>>(result.Value);
            Assert.Equal(1, tables.Count);
        }

            [Fact]
            public async Task GetAllTables_ReturnsNotFoundForInvalidId()
            {
                // Arrange
                var mockTableInterface = new Mock<ITableInterface>();
                mockTableInterface.Setup(repo => repo.GetAllTablesById(It.IsAny<Guid>())).ReturnsAsync((AoTable)null);
                var controller = new AoTablesController(mockTableInterface.Object);


                // Act
                var result = await controller.GetAllTablesById(Guid.NewGuid()) as BadRequestObjectResult;


                // Assert
                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Equal("Data Not Found", result.Value);
            }
        [Fact]
        public async Task UpdateTable_ReturnsOkForValidUpdate()
        {
            // Arrange
            var id = Guid.NewGuid();
            var aoTable = new AoTable { Id = id, Name = "UpdatedTable" };
            var mockTableInterface = new Mock<ITableInterface>();
            mockTableInterface.Setup(repo => repo.IsExists(id)).ReturnsAsync(true);
            mockTableInterface.Setup(repo => repo.UpdateTable(id, aoTable)).ReturnsAsync(true);
            var controller = new AoTablesController(mockTableInterface.Object);

            // Act
            var result = await controller.UpdateTable(id, aoTable) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
            okResult.Value.Should().BeEquivalentTo(new { status = "Success" });
        }

        [Fact]
        public async Task UpdateTable_ReturnsBadRequestForInvalidId()
        {
            // Arrange
            var mockTableInterface = new Mock<ITableInterface>();
            var controller = new AoTablesController(mockTableInterface.Object);
            var id = Guid.NewGuid();
            var aoTable = new AoTable { Id = Guid.NewGuid(), Name = "Table1" };

            // Act
            var result = await controller.UpdateTable(id, aoTable) as BadRequestResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
        }

        [Fact]
        public async Task UpdateTable_ReturnsBadRequestForFailedUpdate()
        {
            // Arrange
            var id = Guid.NewGuid();
            var aoTable = new AoTable { Id = id, Name = "UpdatedTable" };
            var mockTableInterface = new Mock<ITableInterface>();
            mockTableInterface.Setup(repo => repo.IsExists(id)).ReturnsAsync(true);
            mockTableInterface.Setup(repo => repo.UpdateTable(id, aoTable)).ReturnsAsync(false);
            var controller = new AoTablesController(mockTableInterface.Object);

            // Act
            var result = await controller.UpdateTable(id, aoTable) as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Equal("Update failed", result.Value);
        }

        [Fact]
        public async Task AddTable_ReturnsOkForValidTable()
        {
            // Arrange
            var aoTable = new AoTable { Id = Guid.NewGuid(), Name = "NewTable" };
            var mockTableInterface = new Mock<ITableInterface>();
            mockTableInterface.Setup(repo => repo.AddTable(aoTable)).ReturnsAsync(aoTable);
            var controller = new AoTablesController(mockTableInterface.Object);

            // Act
            var result = await controller.AddTable(aoTable) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            var returnedTable = Assert.IsType<AoTable>(result.Value);
            Assert.Equal(aoTable.Id, returnedTable.Id);
        }

        [Fact]
        public async Task AddTable_ReturnsBadRequestForNullTable()
        {
            // Arrange
            var mockTableInterface = new Mock<ITableInterface>();
            var controller = new AoTablesController(mockTableInterface.Object);

            // Act
            var result = await controller.AddTable(null) as BadRequestResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
        }

        [Fact]
        public async Task DeleteTable_ReturnsOkForValidId()
        {
            // Arrange
            var id = Guid.NewGuid();
            var mockTableInterface = new Mock<ITableInterface>();
            mockTableInterface.Setup(repo => repo.IsExists(id)).ReturnsAsync(true);
            var controller = new AoTablesController(mockTableInterface.Object);

            // Act
            var result = await controller.DeleteTable(id) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            okResult.Value.Should().BeEquivalentTo(new { status = "Deleted" });
        }

        [Fact]
        public async Task DeleteTable_ReturnsBadRequestForInvalidId()
        {
            // Arrange
            var id = Guid.NewGuid();
            var mockTableInterface = new Mock<ITableInterface>();
            mockTableInterface.Setup(repo => repo.IsExists(id)).ReturnsAsync(false);
            var controller = new AoTablesController(mockTableInterface.Object);

            // Act
            var result = await controller.DeleteTable(id) as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Equal("Id not found", result.Value);
        }
    }

}