import axios from 'axios';

const API_URL = '/api/urls';

const urlService = {
  /**
   * Rút gọn một URL
   * @param {string} longUrl - URL đầy đủ cần rút gọn
   * @returns {Promise<object>} - Object chứa thông tin về URL đã rút gọn
   */
  async shortenUrl(longUrl) {
    const response = await axios.post(API_URL, { url: longUrl });
    return response.data;
  },
  
  /**
   * Lấy URL gốc từ mã rút gọn
   * @param {string} code - Mã của URL đã rút gọn
   * @returns {Promise<object>} - Object chứa thông tin về URL gốc
   */
  async getOriginalUrl(code) {
    const response = await axios.get(`${API_URL}/${code}`);
    return response.data;
  },
  
  /**
   * Lấy thống kê của một URL rút gọn
   * @param {string} code - Mã của URL đã rút gọn
   * @returns {Promise<object>} - Object chứa thống kê về URL
   */
  async getUrlStats(code) {
    const response = await axios.get(`${API_URL}/${code}/stats`);
    return response.data;
  }
};

export default urlService; 