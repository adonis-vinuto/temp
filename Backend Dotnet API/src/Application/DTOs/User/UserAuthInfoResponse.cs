namespace Authentication.Models;

public class UserAuthInfoResponse
{
    public List<string> Roles { get; set; } = [];
    public List<string> Organizations { get; set; } = [];
    public string IdUser { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
}