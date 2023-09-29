using ReservationAPI.Adapter.Components;
using ReservationAPI.Ports.In;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IReservationManagement, ReservationManagement>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/doctors", async (IReservationManagement reservationManagement) => await reservationManagement.GetDoctors());

app.MapGet("/doctors/{email}", async (IReservationManagement reservationManagement, string email) => await reservationManagement.GetAvailability(email));

app.Run();
