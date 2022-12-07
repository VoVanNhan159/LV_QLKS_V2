using OfficeOpenXml;
using OfficeOpenXml.Style;
using ShareModel;
using ShareModel.Custom;

namespace LV_QLKS.Service
{
    public class DiscountdetailService
    {
        HttpClient Http = new HttpClient();
        string baseurl = "https://localhost:7282/api/Discountdetails";
        public async Task<Discountdetail> GetDiscountdetail(int discountId, int roomId)
        {
            return await Http.GetFromJsonAsync<Discountdetail>(baseurl + "/" + discountId + "/" + roomId);
        }
        //Lấy tất cả Discountdetail của Discount
        public async Task<List<Discountdetail>> GetAllDiscountdetailOfDiscount(int id)
        {
            return await Http.GetFromJsonAsync<List<Discountdetail>>(baseurl + "/GetAllDiscountdetailOfDiscount/" + id);
        }
        public async Task<List<Discountdetail>> GetAllDiscountdetail()
        {
            return await Http.GetFromJsonAsync<List<Discountdetail>>(baseurl);
        }
        //Lấy tất cả Discountdetail của Discount
        public async Task<List<Discountdetail>> GetAllDiscountdetailOfOwner(string phone)
        {
            return await Http.GetFromJsonAsync<List<Discountdetail>>(baseurl + "/GetAllDiscountdetailOfOwner/" + phone);
        }
        //Lấy tất cả Discountdetail còn hạn
        public async Task<List<Discountdetail>> GetAllDiscountdetailActive()
        {
            return await Http.GetFromJsonAsync<List<Discountdetail>>(baseurl + "/GetAllDiscountdetailActive/");
        }
        //Lấy tất cả Discountdetail còn hạn của owner
        public async Task<List<Discountdetail>> GetAllDiscountdetailActiveOfOwner(string phone)
        {
            return await Http.GetFromJsonAsync<List<Discountdetail>>(baseurl + "/GetAllDiscountdetailActiveOfOwner/" + phone);
        }
        //Lấy tất cả Discountdetail còn hạn
        public async Task<List<Discountdetail>> GetAllDiscountdetailActiveDate(DateTime dateStart, DateTime dateEnd)
        {
            LV_QLKSContext _context = new LV_QLKSContext();
            List<Discountdetail> discountDetails = new List<Discountdetail>();
            var discounts = _context.Discounts.ToList();
            foreach (var item in discounts)
            {
                if ((item.DiscountDatestart <= dateStart && item.DiscountDateend >= dateEnd) || item.DiscountDatestart <= dateStart && item.DiscountDateend >= dateStart)
                {
                    var discountDetailsTemp = _context.Discountdetails.Where(dd => dd.DiscountId == item.DiscountId).ToList();
                    discountDetails.AddRange(discountDetailsTemp);
                }
            }
            return discountDetails;
        }
        //Lấy tất cả discoutdetail khi ngày bắt đầu nằm ở giữa
        public async Task<List<Discountdetail>> GetAllDiscountdetailActiveDay(DateTime dateStart)
        {
            LV_QLKSContext _context = new LV_QLKSContext();
            List<Discountdetail> discountDetails = new List<Discountdetail>();
            var discounts = _context.Discounts.ToList();
            foreach (var item in discounts)
            {
                if (item.DiscountDatestart <= dateStart && item.DiscountDateend >= dateStart)
                {
                    var discountDetailsTemp = _context.Discountdetails.Where(dd => dd.DiscountId == item.DiscountId).ToList();
                    discountDetails.AddRange(discountDetailsTemp);
                }
            }
            return discountDetails;
        }
        //Add Discountdetail
        public async Task<DiscountDetail_Custom> AddDiscountDetail(DiscountDetail_Custom discount)
        {
            var response = await Http.PostAsJsonAsync(baseurl + "/", discount);
            return await response.Content.ReadFromJsonAsync<DiscountDetail_Custom>();
        }
        //Delete
        public async Task<int> DeleteDicountDetail(int discountId, int roomId)
        {
            var res = await Http.DeleteAsync(baseurl + "/" + discountId + "/" + roomId);
            if (res.IsSuccessStatusCode)
            {
                return 1;
            }
            return 0;
        }
        //Update
        public async Task<DiscountDetail_Custom> UpdateDiscountDetail(DiscountDetail_Custom DiscountDetail_Custom)
        {
            var res = await Http.PutAsJsonAsync(baseurl + "/" + DiscountDetail_Custom.DiscountId + "/" + DiscountDetail_Custom.RoomId, DiscountDetail_Custom);
            if (res != null)
            {
                var ress = await res.Content.ReadFromJsonAsync<DiscountDetail_Custom>();
                return ress;
            }
            return null;
        }
        //Export
        public async Task<byte[]> ExportRoomToExcel(int id)
        {
            var discountdetails = new List<Discountdetail>();
            try
            {
                discountdetails = await GetAllDiscountdetailOfDiscount(id);
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
            }
            var stream = new MemoryStream();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            // ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(stream))
            {
                var workSheet = package.Workbook.Worksheets.Add("Sheet1");

                // mutual
                workSheet.Row(1).Height = 20;
                workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Row(1).Style.Font.Bold = true;
                workSheet.Cells[1, 1].Value = "Mã khuyến mãi";
                workSheet.Cells[1, 2].Value = "Mã phòng";
                workSheet.Cells[1, 3].Value = "Tỉ lệ khuyến mãi";
                int recordIndex = 2;
                foreach (var item in discountdetails)
                {
                    workSheet.Cells[recordIndex, 1].Value = item.DiscountId;
                    workSheet.Cells[recordIndex, 2].Value = item.RoomId;
                    workSheet.Cells[recordIndex, 3].Value = item.Percent;
                    recordIndex++;
                }

                return package.GetAsByteArray();
            }
        }
    }
}
