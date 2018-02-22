﻿using System;
using LinqToDB;
using LinqToDB.Mapping;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Backend.Entities
{
    public class LeaveRequest : BaseEntity
    {
        public LeaveRequest()
        {
        }

        public LeaveRequest(DateTime startDate, DateTime endDate)
        {
            StartDate = startDate;
            EndDate = endDate;
        }

        public Guid PersonId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Reason { get; set; }
        public bool? Approved { get; set; }

        /// <summary>
        /// Id of the person who approved the leave request
        /// </summary>
        public Guid? ApprovedById { get; set; }

        [Column(DataType = DataType.VarChar)]
        public LeaveType Type { get; set; }

        public DateTime CreatedDate { get; set; }
    }

    [Table("LeaveRequest", IsColumnAttributeRequired = false)]
    public class LeaveRequestWithNames : LeaveRequest
    {
        [Column(SkipOnInsert = true, SkipOnUpdate = true, IsColumn = false)]
        public string RequesterName { get; set; }

        [Column(SkipOnInsert = true, SkipOnUpdate = true, IsColumn = false)]
        public string ApprovedByName { get; set; }
    }


    [JsonConverter(typeof(StringEnumConverter))]
    public enum LeaveType
    {
        [MapValue("Vacation")]
        Vacation,

        [MapValue("Sick")]
        Sick,

        [MapValue("Personal")]
        Personal,

        [MapValue("Funeral")]
        Funeral,

        [MapValue("Maternity")]
        Maternity,

        [MapValue("Paternity")]
        Paternity,

        [MapValue("Buisness")]
        Buisness,

        [MapValue("Compensation")]
        Compensation,

        [MapValue("Other")]
        Other,

        [MapValue("NonPaid")]
        NonPaid,
    }
}