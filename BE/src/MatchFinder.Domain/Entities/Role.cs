﻿using MatchFinder.Domain.Models;

namespace MatchFinder.Domain.Entities
{
    public class Role : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ICollection<User>? Users { get; set; }
    }
}