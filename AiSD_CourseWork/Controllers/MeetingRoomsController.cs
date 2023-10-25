using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AiSD_CourseWork.Data;
using AiSD_CourseWork.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Humanizer;

namespace AiSD_CourseWork.Controllers
{
    public class MeetingRoomsController : Controller
    {
        private readonly MeetingRoomContext _context;

        public MeetingRoomsController(MeetingRoomContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(User model)
        {
            if (ModelState.IsValid)
            {
                User? user = await _context.Users.FirstOrDefaultAsync(u => u.Name == model.Name && u.Password == model.Password);
                if (user != null)
                {
                    var meetingRoom = await _context.MeetingRooms
                        .Include(m => m.IdNavigation)
                        .FirstOrDefaultAsync(m => m.Id == user.Id);

                    if (meetingRoom == null)
                    {
                        return NotFound();
                    }

                    return RedirectToAction("Index", new { user.Id });
                }
                
            }
            ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            return View(model);
        }

        public async Task<IActionResult> Index(int Id)
        {
            ViewData["id"] = Id;    
            var meetingRoomContext = _context.MeetingRooms.Where(m => m.Id == Id).Include(m => m.IdNavigation).OrderBy(m => m.MeetingDate);
            return View(await meetingRoomContext.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Index(int Id, string? Members)
        {
            ViewData["id"] = Id;
            var meetingRoomContext = _context.MeetingRooms.Where(m => m.Id == Id).Include(m => m.IdNavigation).OrderBy(m => m.MeetingDate);
            if(Members != null && Members != "")
                meetingRoomContext = _context.MeetingRooms.Where(m => m.Id == Id).Where(m => m.Members.Contains(Members)).Include(m => m.IdNavigation).OrderBy(m => m.MeetingDate);
            return View(await meetingRoomContext.ToListAsync());
        }

        public async Task<IActionResult> IndexPrev(int Id)
        {
            ViewData["id"] = Id;
            var meetingRoomContext = _context.MeetingRooms.Where(m => m.Id == Id).Include(m => m.IdNavigation).OrderBy(m => m.MeetingDate);
            return View(await meetingRoomContext.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> IndexPrev(int Id, string? Members)
        {
            ViewData["id"] = Id;
            var meetingRoomContext = _context.MeetingRooms.Where(m => m.Id == Id).Include(m => m.IdNavigation).OrderBy(m => m.MeetingDate);
            if (Members != null && Members != "")
                meetingRoomContext = _context.MeetingRooms.Where(m => m.Id == Id).Where(m => m.Members.Contains(Members)).Include(m => m.IdNavigation).OrderBy(m => m.MeetingDate);
            return View(await meetingRoomContext.ToListAsync());
        }

        public async Task<IActionResult> IndexNext(int Id)
        {
            ViewData["id"] = Id;
            var meetingRoomContext = _context.MeetingRooms.Where(m => m.Id == Id).Include(m => m.IdNavigation).OrderBy(m => m.MeetingDate);
            return View(await meetingRoomContext.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> IndexNext(int Id, string? Members)
        {
            ViewData["id"] = Id;
            var meetingRoomContext = _context.MeetingRooms.Where(m => m.Id == Id).Include(m => m.IdNavigation).OrderBy(m => m.MeetingDate);
            if (Members != null && Members != "")
                meetingRoomContext = _context.MeetingRooms.Where(m => m.Id == Id).Where(m => m.Members.Contains(Members)).Include(m => m.IdNavigation).OrderBy(m => m.MeetingDate);
            return View(await meetingRoomContext.ToListAsync());
        }

        // GET: MeetingRooms/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.MeetingRooms == null)
            {
                return NotFound();
            }

            var meetingRoom = await _context.MeetingRooms
                .Include(m => m.IdNavigation)
                .FirstOrDefaultAsync(m => m.MeetingId == id);
            if (meetingRoom == null)
            {
                return NotFound();
            }

            return View(meetingRoom);
        }

        public IActionResult Create(int Id)
        {
            ViewData["id"] = Id;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MeetingId,MeetingName,MeetingDate,TimeFrom,TimeTo,Members,Id")] MeetingRoom meetingRoom, int Id)
        {
            var a = _context.MeetingRooms.Where(m => m.Id == Id).Where(m => m.MeetingDate == meetingRoom.MeetingDate).Include(m => m.IdNavigation);
            bool intersect = false, b;
            foreach (var item in a)
            {
                b = Math.Max(item.TimeFrom.TotalMinutes, meetingRoom.TimeFrom.TotalMinutes) <= Math.Min(item.TimeTo.TotalMinutes, meetingRoom.TimeTo.TotalMinutes);
                if (b == true) intersect = true;
            }
            ViewData["id"] = Id;
            if (!intersect)
            {
                if ((meetingRoom.TimeTo - meetingRoom.TimeFrom).TotalMinutes >= 30 && (meetingRoom.TimeTo - meetingRoom.TimeFrom).Hours < 24)
                {
                    _context.Add(meetingRoom);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", new { Id });
                }
                else
                    ModelState.AddModelError("", "Минимальный промежуток бронирования - 30 минут, максимальный - 24 часа.");
            }
            else
                ModelState.AddModelError("", "В это время уже назначена другая встреча.");
            return View(meetingRoom);
        }

        // GET: MeetingRooms/Edit/5
        public async Task<IActionResult> Edit(int? meetingId)
        {
            if (meetingId == null || _context.MeetingRooms == null)
            {
                return NotFound();
            }

            var meetingRoom = await _context.MeetingRooms.FindAsync(meetingId);
            if (meetingRoom == null)
            {
                return NotFound();
            }
            return View(meetingRoom);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int meetingId, [Bind("MeetingId,MeetingName,MeetingDate,TimeFrom,TimeTo,Members,Id")] MeetingRoom meetingRoom)
        {
            if (meetingId != meetingRoom.MeetingId)
            {
                return NotFound();
            }
            try
            {
                _context.Update(meetingRoom);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", new { meetingRoom.Id });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MeetingRoomExists(meetingRoom.MeetingId))
                {
                    return NotFound();
                }
                else
                {
                    return View(meetingRoom);
                }
            }
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.MeetingRooms == null)
            {
                return NotFound();
            }

            var meetingRoom = await _context.MeetingRooms
                .Include(m => m.IdNavigation)
                .FirstOrDefaultAsync(m => m.MeetingId == id);
            if (meetingRoom == null)
            {
                return NotFound();
            }

            return View(meetingRoom);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.MeetingRooms == null)
            {
                return Problem("Entity set 'MeetingRoomContext.MeetingRooms'  is null.");
            }
            var meetingRoom = await _context.MeetingRooms.FindAsync(id);
            if (meetingRoom != null)
            {
                _context.MeetingRooms.Remove(meetingRoom);
                
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", new { meetingRoom.Id });
        }

        private bool MeetingRoomExists(int id)
        {
          return (_context.MeetingRooms?.Any(e => e.MeetingId == id)).GetValueOrDefault();
        }
    }
}
