using AutoMapper;
using FluentValidation;
using MagicVilla_CouponAPI;
using MagicVilla_CouponAPI.Data;
using MagicVilla_CouponAPI.Models;
using MagicVilla_CouponAPI.Models.DTO;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(MappingConfig));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.MapGet("api/coupon", (ILogger<Program> _logger) =>
{
  _logger.Log(LogLevel.Information, "Getting all Coupons");
  return Results.Ok(CouponStore.couponList);
})
.WithName("GetCoupons")
.Produces<IEnumerable<Coupon>>(200);

app.MapGet("api/coupon/{id:int}", (int id) =>
{
  var coupon = CouponStore
    .couponList
    .FirstOrDefault(c => c.Id == id);

  return Results.Ok(coupon);
})
.WithName("GetCoupon")
.Produces<Coupon>(200);

app.MapPost("api/coupon", (IMapper _mapper,
  IValidator<CouponCreateDTO> _validation,
  [FromBody] CouponCreateDTO coupon_C_DTO) =>
{
  var validationResult = _validation.ValidateAsync(coupon_C_DTO).GetAwaiter().GetResult();

  if (!validationResult.IsValid)
  {
    return Results.BadRequest(validationResult.Errors.FirstOrDefault().ToString());
  }
  if (CouponStore.couponList.FirstOrDefault(c => c.Name.ToLower() == coupon_C_DTO.Name.ToLower()) != null)
  {
    return Results.BadRequest("Coupon Name already exists");
  }

  Coupon coupon = _mapper.Map<Coupon>(coupon_C_DTO);

  coupon.Id = CouponStore
    .couponList
    .OrderByDescending(c => c.Id)
    .FirstOrDefault()
    .Id + 1;
  CouponStore.couponList.Add(coupon);

  CouponDTO couponDTO = _mapper.Map<CouponDTO>(coupon);

  return Results.CreatedAtRoute("GetCoupon", new { id = coupon.Id }, couponDTO);
  //return Results.Created($"api/coupon/{coupon.Id}", coupon);
})
.WithName("CreateCoupon")
.Accepts<CouponCreateDTO>("application/json")
.Produces<CouponDTO>(201).Produces(400); // These are the possible status code that our endpoint can return

app.MapPut("api/coupon", () =>
{

});

app.MapDelete("api/coupon/{id:int}", (int id) =>
{

});

app.UseHttpsRedirection();
app.Run();
