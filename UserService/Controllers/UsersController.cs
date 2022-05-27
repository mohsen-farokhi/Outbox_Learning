﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;
using UserService.Data;
using UserService.Entities;

namespace UserService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly UserServiceContext _context;

    public UsersController(UserServiceContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUser()
    {
        return await _context.User.ToListAsync();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutUser(int id, User user)
    {
        using var transaction = _context.Database.BeginTransaction();

        _context.Entry(user).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        var integrationEventData = JsonConvert.SerializeObject(new
        {
            id = user.ID,
            newname = user.Name,
        });
        _context.IntegrationEventOutbox.Add(
            new IntegrationEvent()
            {
                Event = "user.update",
                Data = integrationEventData
            });

        _context.SaveChanges();
        transaction.Commit();

        return NoContent();
    }

    [HttpPost]
    public async Task<ActionResult<User>> PostUser(User user)
    {
        using var transaction = _context.Database.BeginTransaction();
        _context.User.Add(user);
        _context.SaveChanges();

        var integrationEventData = JsonConvert.SerializeObject(new
        {
            id = user.ID,
            name = user.Name
        });

        _context.IntegrationEventOutbox.Add(
            new IntegrationEvent()
            {
                Event = "user.add",
                Data = integrationEventData
            });

        _context.SaveChanges();
        transaction.Commit();

        return CreatedAtAction("GetUser", new { id = user.ID }, user);
    }

    private void PublishToMessageQueue
            (string integrationEvent, string eventData)
    {
        // TOOO: Reuse and close connections and channel, etc, 
        var factory = new ConnectionFactory();

        var connection = factory.CreateConnection();

        var channel = connection.CreateModel();

        var body = Encoding.UTF8.GetBytes(eventData);

        channel.BasicPublish
            (exchange: "user",
            routingKey: integrationEvent,
            basicProperties: null,
            body: body);
    }
}
