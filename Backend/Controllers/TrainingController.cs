﻿using System;
using System.Collections.Generic;
using System.Linq;
using Backend.Entities;
using Backend.Services;
using LinqToDB;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    public class TrainingController : Controller
    {
        private TrainingService _trainingService;

        public TrainingController(TrainingService trainingService)
        {
            _trainingService = trainingService;
        }

        [HttpGet]
        public IList<TrainingRequirement> List()
        {
            return _trainingService.TrainingRequirements;
        }

        [HttpGet("{id}")]
        public TrainingRequirement Get(Guid id)
        {
            return _trainingService.GetById(id);
        }

        [HttpPost]
        public TrainingRequirement Save([FromBody] TrainingRequirement requirement)
        {
            _trainingService.Save(requirement);
            return requirement;
        }

        [HttpDelete("{id}")]
        public void Delete(Guid id)
        {
            _trainingService.Delete(id);
        }

        [HttpGet("staff/{year}")]
        public IList<StaffTraining> StaffTrainings(int year)
        {
            var startDate = new DateTime(year, 8, 1);
            var endDate = startDate.AddMonths(9);
            return _trainingService.StaffTraining.Where(training =>
                training.CompletedDate != null && training.CompletedDate.Between(startDate, endDate)).ToList();
        }

        [HttpPost("staff")]
        public StaffTraining Save([FromBody] StaffTraining staffTraining)
        {
            _trainingService.Save(staffTraining);
            return staffTraining;
        }

        [HttpPost("staff/allComplete")]
        public IActionResult MarkAllComplete([FromBody] List<Guid> staffIds, Guid? requirementId, DateTime? completeDate)
        {
            if (completeDate == null) throw new ArgumentNullException(nameof(completeDate));
            if (!requirementId.HasValue || requirementId.Value == Guid.Empty) throw new ArgumentNullException(nameof(requirementId));
            _trainingService.MarkAllComplete(staffIds, requirementId.Value, completeDate.Value);
            return Ok();
        }
    }
}