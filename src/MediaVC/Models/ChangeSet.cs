using System;
using System.Collections.Generic;

namespace MediaVC.Models
{
    public class ChangeSet
    {
        public Guid Id { get; set; }

        public string Message { get; set; } = string.Empty;

        public List<Tag> Tags { get; set; } = new();

        public List<Difference> Differences { get; set; } = new();
    }
}
