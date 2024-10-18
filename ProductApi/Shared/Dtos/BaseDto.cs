﻿namespace Shared.Dtos;

public class BaseDto
{
    public int Id { get; set; }
    public DateTime Created { get; set; } = DateTime.Now;
    public DateTime? Updated { get; set; }
}
