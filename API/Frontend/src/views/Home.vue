<template>
  <div class="row justify-content-center">
    <div class="col-md-8">
      <div class="card shadow-sm">
        <div class="card-body">
          <h2 class="card-title text-center mb-4">Shorten Your URL</h2>
          
          <div v-if="error" class="alert alert-danger">
            {{ error }}
          </div>
          
          <form @submit.prevent="shortenUrl">
            <div class="mb-3">
              <label for="url" class="form-label">Enter a long URL:</label>
              <div class="input-group">
                <input
                  type="url"
                  id="url"
                  v-model="longUrl"
                  class="form-control"
                  placeholder="https://example.com/very/long/url"
                  required
                />
                <button type="submit" class="btn btn-primary" :disabled="isLoading">
                  <span v-if="isLoading" class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>
                  Shorten
                </button>
              </div>
              <div class="form-text">Enter a valid URL including http:// or https://</div>
            </div>
          </form>
          
          <div v-if="shortUrl" class="mt-4">
            <div class="card bg-light">
              <div class="card-body">
                <h5 class="card-title">Your shortened URL:</h5>
                <div class="input-group mb-3">
                  <input type="text" class="form-control" :value="shortUrl" readonly />
                  <button class="btn btn-outline-secondary" type="button" @click="copyToClipboard">
                    <i class="bi bi-clipboard"></i> Copy
                  </button>
                </div>
                <div class="d-flex justify-content-between">
                  <a :href="shortUrl" target="_blank" class="btn btn-sm btn-outline-primary">
                    <i class="bi bi-box-arrow-up-right"></i> Open
                  </a>
                  <router-link :to="'/stats/' + shortCode" class="btn btn-sm btn-outline-info">
                    <i class="bi bi-graph-up"></i> View Stats
                  </router-link>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import urlService from '../services/urlService';

export default {
  name: 'Home',
  data() {
    return {
      longUrl: '',
      shortUrl: '',
      shortCode: '',
      isLoading: false,
      error: null
    };
  },
  methods: {
    async shortenUrl() {
      this.error = null;
      this.isLoading = true;
      
      try {
        const response = await urlService.shortenUrl(this.longUrl);
        this.shortUrl = response.shortUrl;
        this.shortCode = response.code;
      } catch (err) {
        this.error = err.response?.data?.message || 'An error occurred while shortening the URL.';
      } finally {
        this.isLoading = false;
      }
    },
    copyToClipboard() {
      navigator.clipboard.writeText(this.shortUrl)
        .then(() => {
          alert('URL copied to clipboard!');
        })
        .catch(err => {
          console.error('Failed to copy text: ', err);
        });
    }
  }
};
</script> 