﻿using System;
using System.Collections.Generic;
using Backend.Entities;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    public class AttachmentController : MyController
    {
        private readonly AttachmentService _attachmentService;

        public AttachmentController(AttachmentService attachmentService)
        {
            _attachmentService = attachmentService;
        }

        [HttpGet("on/{attachedToId}")]
        public IList<Attachment> List(Guid attachedToId)
        {
            return _attachmentService.AttachmentsByAttachedId(attachedToId);
        }

        [HttpPost]
        [Authorize(Roles = "admin,hr")]
        public Attachment Attach([FromBody] Attachment attachment)
        {
            _attachmentService.Save(attachment);
            return attachment;
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin,hr")]
        public IActionResult Delete(Guid id)
        {
            _attachmentService.Delete(id);
            return Ok();
        }
    }
}