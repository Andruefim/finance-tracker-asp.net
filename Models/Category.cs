﻿using AngularWithASP.Server.Interfaces;

namespace AngularWithASP.Server.Models;

public class Category : IUserOwned
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Type { get; set; }
    public string? UserId { get; set; }

}
