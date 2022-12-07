using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ShareModel;
using ShareModel.Custom;

namespace LV_QLKS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly LV_QLKSContext _context;

        public RoomsController(LV_QLKSContext context)
        {
            _context = context;
        }

        // GET: api/Rooms
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Room>>> GetRooms()
        {
          if (_context.Rooms == null)
          {
              return NotFound();
          }
            return await _context.Rooms.ToListAsync();
        }
        // GET: api/Rooms/5
        [HttpGet("GetAllRoomOfOwner/{phone}")]
        [DisableRequestSizeLimit]
        public async Task<ActionResult<IEnumerable<Room>>> GetAllRoomOfOwner(string phone)
        {
            if (_context.Rooms == null)
            {
                return NotFound();
            }
            var room = await _context.Rooms
                .Include(r=>r.Floor)
                .Include(r=>r.Tor)
                .Include(r=>r.Hotel)
                .Where(r => r.Hotel.UserPhone == phone).ToListAsync();
            
            if (room == null)
            {
                return NotFound();
            }

            return room;
        }

        [HttpGet("GetAllRoomOfHotel/{id}")]
        [DisableRequestSizeLimit]
        public async Task<ActionResult<IEnumerable<Room>>> GetAllRoomOfHotel(int id)
        {
            if (_context.Rooms == null)
            {
                return NotFound();
            }
            var room = await _context.Rooms.Where(r => r.Hotel.HotelId == id).ToListAsync();

            if (room == null)
            {
                return NotFound();
            }

            return room;
        }
        [HttpGet("GetAllRoomOfFloorOfHotel/{hotelId}/{floorId}")]
        public async Task<ActionResult<IEnumerable<Room>>> GetAllRoomOfFloorOfHotel(int hotelId, int floorId)
        {
            if (_context.Rooms == null)
            {
                return NotFound();
            }
            var room = await _context.Rooms
                .Include(r => r.Hotel)
                .Include(r => r.Floor)
                .Include(r => r.Tor)
                .Include(r => r.ImageRooms)
                .Where(r => r.Hotel.HotelId == hotelId && r.FloorId == floorId).ToListAsync();

            if (room == null)
            {
                return NotFound();
            }

            return room;
        }
        // GET: api/Rooms/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Room>> GetRoom(int id)
        {
          if (_context.Rooms == null)
          {
              return NotFound();
          }
            var room = await _context.Rooms
                .Include(r=>r.Hotel)
                .Include(r=>r.Floor)
                .Include(r=>r.Tor)
                .Include(r=>r.ImageRooms)
                .FirstAsync(r=>r.RoomId == id);

            if (room == null)
            {
                return NotFound();
            }

            return room;
        }

        // PUT: api/Rooms/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRoom(int id, Room_Custom room)
        {
            if (id != room.RoomId)
            {
                return BadRequest();
            }
            Room roomTemp = new Room();
            roomTemp.RoomStatus = room.RoomStatus;
            roomTemp.RoomName = room.RoomName;
            roomTemp.RoomDescription = room.RoomDescription;
            roomTemp.RoomId = room.RoomId;
            roomTemp.TorId = room.TorId;
            roomTemp.HotelId = room.HotelId;
            roomTemp.FloorId = room.FloorId;
            _context.Entry(roomTemp).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoomExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetRoom", new { id = roomTemp.RoomId }, roomTemp);
        }

        // POST: api/Rooms
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Room_Custom>> PostRoom(Room_Custom room)
        {
          if (_context.Rooms == null)
          {
              return Problem("Entity set 'LV_QLKSContext.Rooms'  is null.");
          }
            var roomTemp = new Room();
            roomTemp.FloorId = room.FloorId;
            roomTemp.TorId = room.TorId;
            roomTemp.HotelId = room.HotelId;
            roomTemp.RoomName = room.RoomName;
            roomTemp.RoomDescription = room.RoomDescription;
            roomTemp.RoomStatus = true;
            _context.Rooms.Add(roomTemp);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRoom", new { id = roomTemp.RoomId }, roomTemp);
        }

        // DELETE: api/Rooms/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            if (_context.Rooms == null)
            {
                return NotFound();
            }
            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RoomExists(int id)
        {
            return (_context.Rooms?.Any(e => e.RoomId == id)).GetValueOrDefault();
        }

        [HttpGet("RateOfRoom/{id}")]
        public string RateOfRoom(int id)
        {
            var listRate = _context.Customerreviews.Where(c => c.RoomId == id).ToList();
            double totalStart = 0;
            foreach (var item in listRate)
            {
                totalStart += (double)item.CrStar;
            }
            double res = totalStart / listRate.Count();
            return res.ToString();
        }
        //Tìm phòng theo mã khách sạn
        [HttpGet("GetListRoomFilter")]
        public List<Room> GetListRoomFilter(int hotelId, DateTime dayStart, DateTime dayEnd, int capacity)
        {
            var listRoomOfHotel = _context.Rooms.Include(r => r.Tor).Where(r => r.HotelId == hotelId).ToList();
            var orderRoomDetail = _context.Orderroomdetails.Where(r => listRoomOfHotel.Select(l => l.RoomId).Contains(r.RoomId)).ToList();
            var orderRoom = _context.Orderrooms.Where(r => orderRoomDetail.Select(odd => odd.OrderroomId).Contains(r.OrderroomId)).ToList();
            List<Room> list = new List<Room>();

            foreach (var item in orderRoom)
            {
                if (item.OrderroomDatestart != dayStart && item.OrderroomDateend != dayEnd && item.OrderroomDatestart > dayStart && item.OrderroomDateend > dayEnd)
                {
                    var roomid = _context.Orderroomdetails.Where(odd => odd.OrderroomId == item.OrderroomId).Select(odd => odd.RoomId).FirstOrDefault();
                    var room = _context.Rooms.Include(r => r.ImageRooms).Where(r=>r.RoomId == roomid).FirstOrDefault();
                    if (room.Tor.TorCapacity == capacity)
                    {
                        list.Add(room);
                    }

                }
            }
            foreach (var item in listRoomOfHotel)
            {
                if (!list.Contains(item))
                {
                    var room = _context.Rooms.Include(r => r.ImageRooms).Where(r => r.RoomId == item.RoomId).SingleOrDefault();
                    if (item.Tor.TorCapacity == capacity)
                    {
                        list.Add(room);
                    }
                }
            }

            return list;
        }
        //Khách sạn còn hạn đăng tin
        [HttpGet("GetAllHotelIsActive")]
        public async Task<List<Hotel>> GetAllHotelIsActive()
        {
            var hotels = new List<Hotel>();
            var hotelsTemp = await _context.Hotels.Include(h => h.Ward).ToListAsync();
            var priceListBRs = await _context.Pricelistbrs.ToListAsync();
            var hotel = new List<Hotel>();
            foreach (var item in hotelsTemp)
            {
                var image = _context.ImageHotels.Where(i => i.HotelId == item.HotelId).FirstOrDefault();
                var hotelAdd = new Hotel();
                hotelAdd = item;
                hotelAdd.ImageHotels.Add(image);
                hotel.Add(hotelAdd);
            }

            var businessregistrations = await _context.Businessregistrations.Include(br => br.Hotel).ToListAsync();
            foreach (var item in businessregistrations)
            {
                var priceListBR = priceListBRs.Single(plbr => plbr.PricelistbrId == item.PricelistbrId);
                if (DateTime.Parse(item.BrDate.ToString()).AddMonths((int)priceListBR.PricelistbrMonth) >= DateTime.Now)
                    hotels.Add(item.Hotel);
            }

            return hotels;
        }
        //tìm phòng theo tỉnh, ngày, số lượng
        [HttpGet("GetListRoomFillter")]
        public List<Room> GetListRoomFillter(string provinceName, DateTime dayStart, DateTime dayEnd, int capacity)
        {
            List<Province> provinces = new List<Province>();
            List<District> districts = new List<District>();
            List<Hotel> hotels = new List<Hotel>();
            var hotelsTemp = GetAllHotelIsActive().Result;
            var provincesTemp = _context.Provinces.ToList();   
            var districtsTemp = _context.Districts.ToList();   

            provinces.AddRange(provincesTemp.Where(pv => RemoveSign4VietnameseString(pv.ProvinceName).ToLower().Contains(RemoveSign4VietnameseString(provinceName).ToLower())));
            districts.AddRange(districtsTemp.Where(dt => RemoveSign4VietnameseString(dt.DistrictName).ToLower().Contains(RemoveSign4VietnameseString(provinceName).ToLower())));
            foreach (var hotel in hotelsTemp)
            {
                if (provinces.Select(p => p.ProvinceId).Contains(hotel.ProvinceId) || districts.Select(d => d.DistrictId).Contains(hotel.DistrictId))
                    hotels.Add(hotel);
                if (RemoveSign4VietnameseString(hotel.HotelName).ToLower().Contains(RemoveSign4VietnameseString(provinceName).ToLower()))
                {
                    if(!hotels.Any(h=>h.HotelId == hotel.HotelId))
                    {
                        hotels.Add(hotel);
                    }
                }
            }

            var listRoomOfHotel = _context.Rooms.Include(r => r.Tor).Where(r => hotels.Select(h=>h.HotelId).Contains(r.HotelId)).ToList();
            var orderRoomDetail = _context.Orderroomdetails.Where(r => listRoomOfHotel.Select(l => l.RoomId).Contains(r.RoomId)).ToList();
            var orderRoom = _context.Orderrooms.Where(r => orderRoomDetail.Select(odd => odd.OrderroomId).Contains(r.OrderroomId)).ToList();
            List<Room> list = new List<Room>();

            foreach (var item in orderRoom)
            {
                if (item.OrderroomDatestart != dayStart && item.OrderroomDateend != dayEnd && item.OrderroomDatestart > dayStart && item.OrderroomDateend > dayEnd)
                {
                    var roomid = _context.Orderroomdetails.Where(odd => odd.OrderroomId == item.OrderroomId).Select(odd => odd.RoomId).FirstOrDefault();
                    var room = _context.Rooms.Include(r => r.ImageRooms).Where(r=>r.RoomId == roomid).FirstOrDefault();
                    if (room.Tor.TorCapacity == capacity)
                    {
                        list.Add(room);
                    }

                }
            }
            foreach (var item in listRoomOfHotel)
            {
                if (!list.Contains(item))
                {
                    var room = _context.Rooms.Include(r => r.ImageRooms).Where(r => r.RoomId == item.RoomId).SingleOrDefault();
                    if (item.Tor.TorCapacity == capacity)
                    {
                        list.Add(room);
                    }
                }
            }

            return list;
        }
        private static readonly string[] VietnameseSigns = new string[]
        {

            "aAeEoOuUiIdDyY",

            "áàạảãâấầậẩẫăắằặẳẵ",

            "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",

            "éèẹẻẽêếềệểễ",

            "ÉÈẸẺẼÊẾỀỆỂỄ",

            "óòọỏõôốồộổỗơớờợởỡ",

            "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",

            "úùụủũưứừựửữ",

            "ÚÙỤỦŨƯỨỪỰỬỮ",

            "íìịỉĩ",

            "ÍÌỊỈĨ",

            "đ",

            "Đ",

            "ýỳỵỷỹ",

            "ÝỲỴỶỸ"
        };
        //Trả về chuỗi không dấu
        public static string RemoveSign4VietnameseString(string str)
        {
            for (int i = 1; i < VietnameseSigns.Length; i++)
            {
                for (int j = 0; j < VietnameseSigns[i].Length; j++)
                    str = str.Replace(VietnameseSigns[i][j], VietnameseSigns[0][i - 1]);
            }
            return str;
        }
    }
}
