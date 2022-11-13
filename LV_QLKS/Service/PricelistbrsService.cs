using ShareModel;
using ShareModel.Custom;

namespace LV_QLKS.Service
{
    public class PricelistbrService
    {
        HttpClient Http = new HttpClient();
        string baseurl = "https://localhost:7282/api/Pricelistbrs";
        public async Task<Pricelistbr> GetPricelistbr(int id)
        {
            return await Http.GetFromJsonAsync<Pricelistbr>(baseurl + "/" + id);
        }
        //Lấy tất cả Businessregistration
        public async Task<List<Pricelistbr>> GetAllPricelistbr()
        {
            return await Http.GetFromJsonAsync<List<Pricelistbr>>(baseurl);
        }
        public async Task<Pricelistbr> UpdatePriceListBr(PriceListBr_Custom priceListBr_Custom)
        {
            var res = await Http.PutAsJsonAsync(baseurl + "/" + priceListBr_Custom.PricelistbrId, priceListBr_Custom);
            if (res != null)
            {
                return await res.Content.ReadFromJsonAsync<Pricelistbr>();
            }
            return null;
        }
        public async Task<Pricelistbr> AddPriceListBr(PriceListBr_Custom priceListBr_Custom)
        {
            var response = await Http.PostAsJsonAsync(baseurl + "/", priceListBr_Custom);
            return await response.Content.ReadFromJsonAsync<Pricelistbr>();
        }
    }
}
