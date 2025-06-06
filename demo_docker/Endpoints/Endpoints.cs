using demo_docker.Common;
using demo_docker.DbServices;
using demo_docker.Dto;
using demo_docker.Models;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
namespace demo_docker.Endpoints
{
    public static class Endpoints
    {
        public static void AddEndpoints(this WebApplication app)
        {
            app.MapPost("/createnotes",async (AddNotes addNotes,AppDbContext _context,IConfiguration _config, IDistributedCache _cache) =>
            {
                if (addNotes != null)
                {
                    Notes notes = new Notes();
                    notes.Message = addNotes.Message;
                    notes.IsPassoword = addNotes.IsPassoword;
                    var options = new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
                        SlidingExpiration = TimeSpan.FromMinutes(2)
                    };
                    if (addNotes.IsPassoword)
                    {
                        notes.Passoword = addNotes.Passoword;
                    }
                
                     var res = await _context.Notes.AddAsync(notes);
                    await _context.SaveChangesAsync();
                                        
                    var url = $"http://localhost:8080/message/{res.Entity.Id}";             
                    return Results.Ok(new { url = url  });
                }
                return Results.BadRequest(new { message="Enter Message" });
            });



            //app.MapGet("/getnote/{Id:guid}", async (Guid Id,string? Password , AppDbContext _context , IDistributedCache _cache) =>
            //{
            //    var cacheKey = $"Note_{Id}";
            //    var cachedNote = await _cache.GetStringAsync(cacheKey);
            //    if (cachedNote != null)
            //    {
            //        var res = JsonSerializer.Deserialize<Notes>(cachedNote);

            //        if (res.IsPassoword)
            //        {
            //            if(Password == null)
            //            {
            //                return Results.BadRequest("Password is Required!");

            //            }else if(Password != res.Passoword)
            //            {
            //                return Results.BadRequest("Invalid!");
            //            }

            //            return res;
            //        }
            //        else
            //        {
            //            return res;
            //        }
            //    }

            //    // get from database
            //    var dbNote = await _context.Notes.FirstOrDefaultAsync(op => op.Id == Id);
            //    if (dbNote != null)
            //    {
            //        if (dbNote.IsPassoword)
            //        {
            //            if (Password == null)
            //            {
            //                return Results.BadRequest("Password is Required!");

            //            }
            //            else if (Password != res.Passoword)
            //            {
            //                return Results.BadRequest("Invalid!");
            //            }

            //            var cacheOptions = new DistributedCacheEntryOptions
            //            {
            //                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
            //            };
            //            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(dbNote), cacheOptions);
            //            return dbNote;
            //        }

            //    }


            //    var cacheOptions = new DistributedCacheEntryOptions
            //    {
            //        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
            //    };
            //    await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(dbNote), cacheOptions);
            //    return dbNote;


            //});



            app.MapGet("/getnote/{Id:guid}", async (HttpContext context) =>
            {
                // Extract parameters from the context
                var Id = Guid.Parse(context.Request.RouteValues["Id"]?.ToString());
                var Password = context.Request.Query["Password"].FirstOrDefault();

                // Get services from DI
                var _context = context.RequestServices.GetRequiredService<AppDbContext>();
                var _cache = context.RequestServices.GetRequiredService<IDistributedCache>();

                var cacheKey = $"Note_{Id}";
                var cachedNote = await _cache.GetStringAsync(cacheKey);
                if (cachedNote != null)
                {
                    var res = JsonSerializer.Deserialize<Notes>(cachedNote);

                    if (res.IsPassoword)
                    {
                        if (Password == null)
                        {
                            var newObj = new
                            {
                                Message = "Password is Required!",
                                IsPassword = true
                            };
                            return Results.BadRequest(newObj);
                        }
                        else if (Password != res.Passoword)
                        {
                            var newObj = new
                            {
                                Message = "Invalid Password!",
                                IsPassword = true
                            };
                            return Results.BadRequest(newObj);
                        }
                        var Obj = _context.Notes.Where(op => op.Id == res.Id).FirstOrDefault();
                        if (Obj != null)
                        {
                            _context.Notes.Remove(Obj);
                            _context.SaveChanges();
                        }
                        await _cache.RemoveAsync(cacheKey);
                        return Results.Ok(res.Message);
                    }
                    else
                    {
                        var Obj = _context.Notes.Where(op => op.Id == res.Id).FirstOrDefault();
                        if (Obj != null)
                        {
                            _context.Notes.Remove(Obj);
                            _context.SaveChanges();
                        }
                        await _cache.RemoveAsync(cacheKey);
                        return Results.Ok(res.Message);
                    }
                }

                // get from database
                var dbNote = await _context.Notes.FirstOrDefaultAsync(op => op.Id == Id);
                if (dbNote == null)
                {
                    return Results.NotFound();
                }

                if (dbNote.IsPassoword)
                {
                    if (Password == null)
                    {
                        var cacheOptions = new DistributedCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
                        };
                        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(dbNote), cacheOptions);
                        var newObj = new
                        {
                            Message = "Password is Required!",
                            IsPassword = true
                        };
                        return Results.BadRequest(newObj);
                    }
                    else if (Password != dbNote.Passoword)
                    {
                        var cacheOptions = new DistributedCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
                        };
                        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(dbNote), cacheOptions);
                        var newObj = new
                        {
                            Message = "Invalid Password!",
                            IsPassword = true
                        };
                        return Results.BadRequest(newObj);
                    }
                }

                var deleteObj = _context.Notes.Where(op=>op.Id == dbNote.Id).FirstOrDefault();
                if(deleteObj != null)
                {
                    _context.Notes.Remove(deleteObj);
                    _context.SaveChanges();
                }
                await _cache.RemoveAsync(cacheKey);

                return Results.Ok(dbNote.Message);
            });


            app.MapGet("/checknote", async (Guid Id, IDistributedCache _cache , AppDbContext _context) =>
            {
                var Note_Id = await _cache.GetStringAsync("Note_Id");

                if(Note_Id == null)
                {
                    var res = _context.Notes.Where(op=>op.Id == Guid.Parse(Note_Id)).FirstOrDefault();
                    if(res== null)
                    {
                        return Results.NotFound(new { message="Not Exist" , exist=false  });
                    }
                    if(res.IsPassoword)
                    {
                        return Results.Ok(new { message = "Password Required", isPassword = true });
                    }
                    return Results.Ok(new { message = "Password Not Reuired", isPassword = false });
                }
                var IsPassword = await _cache.GetStringAsync("IsPassword");
                if(IsPassword=="true")
                {
                    return Results.Ok(new { message = "Password Required", isPassword = true });
                }

                return Results.Ok(new { message = "Password Not Reuired", isPassword = false });


            });
        }


        

    }
}
