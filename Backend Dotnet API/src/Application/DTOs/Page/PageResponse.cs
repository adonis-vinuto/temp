using Application.DTOs.Agent;
using Domain.Entities;
using Domain.Enums;

namespace Application.DTOs.Page;

public class PageResponse
{
    public Guid Id { get; set; }
    public int PageNumber { get; set; }
    public string? Title { get; set; }
    public string? Content { get; set; }
    public string? ResumePage { get; set; }
}