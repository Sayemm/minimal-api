using MagicVilla_CouponAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.MapGet("api/coupon", () =>
{
  return Results.Ok(CouponStore.couponList);
});

app.MapGet("api/coupon/{id:int}", (int id) =>
{
  var coupon = CouponStore
    .couponList
    .FirstOrDefault(c => c.Id == id);

  return Results.Ok(coupon);
});

app.MapPost("api/coupon", () =>
{

});

app.MapPut("api/coupon", () =>
{

});

app.MapDelete("api/coupon/{id:int}", (int id) =>
{

});

app.UseHttpsRedirection();
app.Run();
