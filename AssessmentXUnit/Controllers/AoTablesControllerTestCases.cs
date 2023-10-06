using AutoFixture;
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
using Humanizer;
using Newtonsoft.Json.Linq;
using static Azure.Core.HttpHeader;
using Xunit;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Metadata;

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
        public async Task GetAllTables_ReturnsOk_WhenTablesExists()
        {
            // Arrange
            List<AoTable> aoTables = fixture.CreateMany<AoTable>().ToList();
            tableInterface.Setup(repo => repo.GetAllTables()).ReturnsAsync(aoTables);

            // Act
            var result = await tableController.GetAllTables();

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
            var tables = okResult.Value as List<AoTable>;
            tables.Should().NotBeNull().And.NotBeEmpty();
            Assert.Equal(aoTables.Count, tables.Count);
            tableInterface.Verify(repo => repo.GetAllTables(), Times.Once());
        }

            [Fact]
            public async Task GetAllTables_ReturnsNotFound_WhenInvalidResult()
            {
            // Arrange
            tableInterface.Setup(repo => repo.GetAllTables()).ReturnsAsync(new List<AoTable>());


            // Act
            var result = await tableController.GetAllTables();


            // Assert
            result.Should().NotBeNull();
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errorMessage = Assert.IsType<string>(badRequestResult.Value);
            Assert.Equal("No Table Found", errorMessage);
            tableInterface.Verify(repo => repo.GetAllTables(), Times.Once());
        }

        [Fact]
        public async Task GetAllTables_ExceptionOccurs_WhenReturnsBadRequest()
        {
            // Arrange
            tableInterface.Setup(c => c.GetAllTables()).Throws(new Exception("Something went wrong"));

            // Act
            var result = await tableController.GetAllTables();

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errorMessage = Assert.IsType<string>(badRequestResult.Value);
            Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
            Assert.Equal("Something went wrong", errorMessage);
            tableInterface.Verify(repo => repo.GetAllTables(), Times.Once());
        }


        [Fact]
        public async Task UpdateTable_ReturnsOk_WhenValidTableData()
    {
            // Arrange
            Guid id = fixture.Create<Guid>();
            var updateTable = fixture.Create<AoTable>();
            updateTable.Id = id;
            tableInterface.Setup(v => v.IsExists(id)).ReturnsAsync(true);
            tableInterface.Setup(repo => repo.UpdateTable(updateTable)).ReturnsAsync(true);

            // Act
            var result = await tableController.UpdateTable(updateTable);

            // Assert
            result.Should().NotBeNull();
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
            okResult.Value.Should().BeEquivalentTo(new { status = "Success" });
            tableInterface.Verify(v => v.IsExists(id), Times.Once());
            tableInterface.Verify(b => b.UpdateTable(updateTable), Times.Once());
        }

        [Fact]
        public async Task UpdateTable_ReturnsBadRequest_WhenIdNotFound()
        {
            // Arrange
            Guid id = fixture.Create<Guid>();
            var updateTable = fixture.Create<AoTable>();
            updateTable.Id = id;
            tableInterface.Setup(x => x.IsExists(id)).ReturnsAsync(false);
            tableInterface.Setup(x => x.UpdateTable(updateTable)).ReturnsAsync(false);

            // Act
            var result = await tableController.UpdateTable(updateTable);

            // Assert
            result.Should().NotBeNull();
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errorMessage = Assert.IsType<string>(badRequestResult.Value);
            Assert.Equal("Update Failed! Table not found", errorMessage);
            tableInterface.Verify(v => v.IsExists(id), Times.Once());
            tableInterface.Verify(x => x.UpdateTable(updateTable), Times.Never());
        }

        [Fact]
        public async Task UpdateTable_ReturnsBadRequest_WhenStatusNotSuccess()
        {
            // Arrange
            var id = fixture.Create<Guid>();
            var aoTable = fixture.Create<AoTable>();
            aoTable.Id = id;
            var ReturnData = false;
            tableInterface.Setup(repo => repo.IsExists(id)).ReturnsAsync(true);
            tableInterface.Setup(repo => repo.UpdateTable(aoTable)).ReturnsAsync(ReturnData);


            // Act
            var result = await tableController.UpdateTable(aoTable);

            // Assert
            result.Should().NotBeNull();
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errorMessage = Assert.IsType<string>(badRequestResult.Value);
            Assert.Equal("Update failed", errorMessage);
            tableInterface.Verify(x => x.IsExists(id), Times.Once());
            tableInterface.Verify(x => x.UpdateTable(aoTable), Times.Once());
        }

        [Fact]
        public async Task UpdateTable_ReturnsBadRequest_WhenExceptionOccurs()
        {
            // Arrange
            Guid id = fixture.Create<Guid>();
            var updateTable = fixture.Create<AoTable>();
            updateTable.Id = id;

            tableInterface.Setup(v => v.IsExists(id)).ReturnsAsync(true);
            tableInterface.Setup(b => b.UpdateTable(updateTable)).ThrowsAsync(new Exception("Something went wrong"));

            // Act
            var result = await tableController.UpdateTable(updateTable);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errorMessage = Assert.IsType<string>(badRequestResult.Value);
            result.Should().NotBeNull();
            Assert.Equal("Something went wrong", errorMessage);
            tableInterface.Verify(v => v.IsExists(id), Times.Once());
            tableInterface.Verify(b => b.UpdateTable(updateTable), Times.Once());
        }

        [Fact]
        public async Task AddTable_ReturnsOk_WhenValidTable()
        {
            // Arrange
            var aoTable = fixture.Create<AoTable>();
            var returnData = fixture.Create<AoTable>();
            tableInterface.Setup(repo => repo.AddTable(aoTable)).ReturnsAsync(returnData);

            // Act
            var result = await tableController.AddTable(aoTable);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<OkObjectResult>();
            var okObjectResult = result.As<OkObjectResult>();
            okObjectResult.Value.Should().BeEquivalentTo(returnData);
            tableInterface.Verify(t => t.AddTable(aoTable), Times.Once());
        }
    

        [Fact]
        public async Task AddTable_ReturnsBadRequest_WhenNullTable()
        {
        // Arrange
            AoTable aoTable = null;
            tableInterface.Setup(c => c.AddTable(aoTable)).ReturnsAsync((AoTable)null);

            // Act
            var result = await tableController.AddTable(null) as BadRequestResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            tableInterface.Verify(t => t.AddTable(aoTable), Times.Never());
        }

        [Fact]
        public async Task AddTable_ReturnsBadRequest_WhenExceptionOccurs()
        {
            // Arrange
            var aoTable = fixture.Create<AoTable>();
            tableInterface.Setup(repo => repo.AddTable(aoTable))
                .ThrowsAsync(new Exception("Error message"));

            // Act
            var result = await tableController.AddTable(aoTable);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errorMessage = Assert.IsType<string>(badRequestResult.Value);
            result.Should().NotBeNull();
            tableInterface.Verify(t => t.AddTable(aoTable), Times.Once());
            Assert.Equal("Error message", errorMessage);
        }

        [Fact]
        public async Task DeleteTable_ReturnsOk_WhenTableExits()
        {
            // Arrange
            var id = fixture.Create<Guid>();
            var aoTable = fixture.Create<AoTable>();
            tableInterface.Setup(repo => repo.IsExists(id)).ReturnsAsync(true);
            tableInterface.Setup(repo => repo.DeleteTable(id)).Returns(Task.CompletedTask);

            // Act
            var result = await tableController.DeleteTable(id);

            // Assert
            result.Should().NotBeNull();
            var okResult = Assert.IsType<OkObjectResult>(result);
            okResult.Value.Should().BeEquivalentTo(new { status = "Deleted" });
            tableInterface.Verify(x => x.DeleteTable(id), Times.Once());
        }

        [Fact]
        public async Task DeleteTable_ReturnsBadRequest_WhenTableNotFound()
        {
            // Arrange
            var id = fixture.Create<Guid>();
            tableInterface.Setup(repo => repo.IsExists(id)).ReturnsAsync(false);

            // Act
            var result = await tableController.DeleteTable(id);

            // Assert
            result.Should().NotBeNull();
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().Be("Delete Failed! Table not found");
            tableInterface.Verify(repo => repo.IsExists(id), Times.Once());
            tableInterface.Verify(x => x.DeleteTable(id), Times.Never);
        }
        [Fact]
        public async Task DeleteTable_ReturnsBadRequest_WhenExceptionOccurs()
        {
            // Arrange
            var id = Guid.NewGuid();
            var errorMessage = "An error occurred";
            tableInterface.Setup(repo => repo.IsExists(id)).ReturnsAsync(true);
            tableInterface.Setup(repo => repo.DeleteTable(id)).ThrowsAsync(new Exception(errorMessage));

            // Act
            var result = await tableController.DeleteTable(id);

            // Assert
            result.Should().NotBeNull();
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().Be(errorMessage);
            tableInterface.Verify(repo => repo.IsExists(id), Times.Once());
            tableInterface.Verify(x => x.DeleteTable(id), Times.Once);
        }

        [Fact]
        public async Task GetAllTablesById_ReturnsOk_WhenTablesExists()
        {
            // Arrange
            Guid id = Guid.NewGuid();
            var aoTable = fixture.Create<AoTable>();
            tableInterface.Setup(repo => repo.GetAllTablesById(id)).ReturnsAsync(aoTable);

            // Act
            var result = await tableController.GetAllTablesById(id);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okObjectResult = result as OkObjectResult;
            okObjectResult.Value.Should().Be(aoTable);
            tableInterface.Verify(repo => repo.GetAllTablesById(id), Times.Once());
        }

        [Fact]
        public async Task GetAllTablesById_ReturnsBadRequest_WhenTableNotFound()
        {
            // Arrange
            Guid id = Guid.NewGuid();
            tableInterface.Setup(repo => repo.GetAllTablesById(id)).ReturnsAsync((AoTable?)null);

            // Act
            var result = await tableController.GetAllTablesById(id);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Value.Should().Be("Table Not Found");
            tableInterface.Verify(t => t.GetAllTablesById(id), Times.Once());
        }

        [Fact]
        public async Task GetAllTablesById_ReturnsBadRequest_WhenExceptionOccurs()
        {
            // Arrange
            Guid id = Guid.NewGuid();
            var errorMessage = "An error occurred.";
            tableInterface.Setup(repo => repo.GetAllTablesById(id)).Throws(new Exception(errorMessage));

            // Act
            var result = await tableController.GetAllTablesById(id);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Value.Should().Be(errorMessage);
            Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
            tableInterface.Verify(x => x.DeleteTable(id), Times.Never);
        }
    }

}
