using AutoMapper;
using FluentValidation;
using MagicVilla_CouponAPI;
using MagicVilla_CouponAPI.Data;
using MagicVilla_CouponAPI.Models;
using MagicVilla_CouponAPI.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Net;

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
  APIResponse response = new();
  _logger.Log(LogLevel.Information, "Getting all Coupons");

  response.Result = CouponStore.couponList;
  response.IsSuccess = true;
  response.StatusCode = HttpStatusCode.OK;
  return Results.Ok(response);
})
.WithName("GetCoupons")
.Produces<APIResponse>(200);

app.MapGet("api/coupon/{id:int}", (int id) =>
{
  APIResponse response = new();

  var coupon = CouponStore
    .couponList
    .FirstOrDefault(c => c.Id == id);

  response.Result = coupon;
  response.IsSuccess = true;
  response.StatusCode = HttpStatusCode.OK;
  return Results.Ok(response);
})
.WithName("GetCoupon")
.Produces<APIResponse>(200);

app.MapPost("api/coupon", async (IMapper _mapper,
  IValidator<CouponCreateDTO> _validation,
  [FromBody] CouponCreateDTO coupon_C_DTO) =>
{
  APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };

  var validationResult = await _validation.ValidateAsync(coupon_C_DTO);
  if (!validationResult.IsValid)
  {
    response.Errormessages.Add(validationResult.Errors.FirstOrDefault().ToString());
    return Results.BadRequest(response);
  }
  if (CouponStore.couponList.FirstOrDefault(c => c.Name.ToLower() == coupon_C_DTO.Name.ToLower()) != null)
  {
    response.Errormessages.Add("Coupon Name already exists");
    return Results.BadRequest(response);
  }

  Coupon coupon = _mapper.Map<Coupon>(coupon_C_DTO);

  coupon.Id = CouponStore
    .couponList
    .OrderByDescending(c => c.Id)
    .FirstOrDefault()
    .Id + 1;
  CouponStore.couponList.Add(coupon);

  CouponDTO couponDTO = _mapper.Map<CouponDTO>(coupon);

  response.Result = couponDTO;
  response.IsSuccess = true;
  response.StatusCode = HttpStatusCode.Created;
  return Results.Ok(response);

  // return Results.CreatedAtRoute("GetCoupon", new { id = coupon.Id }, couponDTO);
  // return Results.Created($"api/coupon/{coupon.Id}", coupon);
})
.WithName("CreateCoupon")
.Accepts<CouponCreateDTO>("application/json")
.Produces<APIResponse>(201).Produces(400);

app.MapPut("api/coupon", async (IMapper _mapper,
  IValidator<CouponUpdateDTO> _validation,
  [FromBody] CouponUpdateDTO coupon_U_DTO) =>
{
  APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };

  var validationResult = await _validation.ValidateAsync(coupon_U_DTO);
  if (!validationResult.IsValid)
  {
    response.Errormessages.Add(validationResult.Errors.FirstOrDefault().ToString());
    return Results.BadRequest(response);
  }

  var couponFromStore = CouponStore.couponList.FirstOrDefault(c => c.Id == coupon_U_DTO.Id);
  couponFromStore.IsActive = coupon_U_DTO.IsActive;
  couponFromStore.Name = coupon_U_DTO.Name;
  couponFromStore.Percent = coupon_U_DTO.Percent;
  couponFromStore.LastUpdated = DateTime.Now;

  response.Result = _mapper.Map<CouponDTO>(couponFromStore);
  response.IsSuccess = true;
  response.StatusCode = HttpStatusCode.OK;
  return Results.Ok(response);
})
.WithName("UpdateCoupon")
.Accepts<CouponUpdateDTO>("application/json")
.Produces<APIResponse>(200).Produces(400);

app.MapDelete("api/coupon/{id:int}", (int id) =>
{
  APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };

  var couponFromStore = CouponStore.couponList.FirstOrDefault(c => c.Id == id);
  if (couponFromStore != null)
  {
    CouponStore.couponList.Remove(couponFromStore);
    response.IsSuccess = true;
    response.StatusCode = HttpStatusCode.NoContent;
    return Results.Ok(response);
  }
  else
  {
    response.Errormessages.Add("Invalid Id");
    return Results.BadRequest(response);
  }
});

app.UseHttpsRedirection();
app.Run();
