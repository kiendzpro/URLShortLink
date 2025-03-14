<template>
  <div>
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h2>URL Statistics</h2>
      <router-link to="/" class="btn btn-outline-primary">
        <i class="bi bi-plus-circle"></i> Create New URL
      </router-link>
    </div>

    <div v-if="loading" class="text-center my-5">
      <div class="spinner-border text-primary" role="status">
        <span class="visually-hidden">Loading...</span>
      </div>
      <p class="mt-2">Loading statistics...</p>
    </div>

    <div v-if="error" class="alert alert-danger">
      {{ error }}
    </div>

    <div v-if="!loading && !error && urlData">
      <div class="card mb-4">
        <div class="card-body">
          <h5 class="card-title">URL Details</h5>
          <div class="table-responsive">
            <table class="table">
              <tbody>
                <tr>
                  <th style="width: 150px">Short URL</th>
                  <td>
                    <div class="d-flex align-items-center">
                      <a :href="urlData.shortUrl" target="_blank">{{ urlData.shortUrl }}</a>
                      <button class="btn btn-sm btn-outline-secondary ms-2" @click="copyToClipboard(urlData.shortUrl)">
                        <i class="bi bi-clipboard"></i>
                      </button>
                    </div>
                  </td>
                </tr>
                <tr>
                  <th>Original URL</th>
                  <td>
                    <div class="d-flex align-items-center">
                      <a :href="urlData.originalUrl" target="_blank" class="text-truncate d-inline-block" style="max-width: 600px">
                        {{ urlData.originalUrl }}
                      </a>
                      <button class="btn btn-sm btn-outline-secondary ms-2" @click="copyToClipboard(urlData.originalUrl)">
                        <i class="bi bi-clipboard"></i>
                      </button>
                    </div>
                  </td>
                </tr>
                <tr>
                  <th>Created At</th>
                  <td>{{ formatDate(urlData.createdAt) }}</td>
                </tr>
                <tr>
                  <th>Expires At</th>
                  <td>{{ urlData.expiresAt ? formatDate(urlData.expiresAt) : 'Never' }}</td>
                </tr>
                <tr>
                  <th>Total Clicks</th>
                  <td><span class="badge bg-primary fs-6">{{ urlData.totalClicks }}</span></td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>

      <div class="card">
        <div class="card-body">
          <h5 class="card-title">Recent Clicks</h5>
          
          <div v-if="urlData.clicks && urlData.clicks.length > 0">
            <div class="table-responsive">
              <table class="table table-striped">
                <thead>
                  <tr>
                    <th>Time</th>
                    <th>IP Address</th>
                    <th>Referrer</th>
                    <th>User Agent</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="(click, index) in urlData.clicks" :key="index">
                    <td>{{ formatDate(click.clickedAt) }}</td>
                    <td>{{ click.ipAddress || 'Unknown' }}</td>
                    <td>{{ click.referrer || 'Direct' }}</td>
                    <td class="text-truncate" style="max-width: 200px">{{ click.userAgent || 'Unknown' }}</td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
          
          <div v-else class="text-center py-4">
            <i class="bi bi-graph-down text-muted" style="font-size: 2rem;"></i>
            <p class="text-muted mt-2">No clicks recorded yet</p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import urlService from '../services/urlService';

export default {
  name: 'UrlStats',
  data() {
    return {
      loading: true,
      error: null,
      urlData: null
    };
  },
  mounted() {
    this.fetchUrlStats();
  },
  methods: {
    async fetchUrlStats() {
      this.loading = true;
      const code = this.$route.params.code;
      
      try {
        this.urlData = await urlService.getUrlStats(code);
      } catch (err) {
        this.error = err.response?.data?.message || 'Error loading URL statistics.';
      } finally {
        this.loading = false;
      }
    },
    formatDate(dateString) {
      if (!dateString) return 'N/A';
      
      const date = new Date(dateString);
      return new Intl.DateTimeFormat('en-US', {
        dateStyle: 'medium',
        timeStyle: 'short'
      }).format(date);
    },
    copyToClipboard(text) {
      navigator.clipboard.writeText(text)
        .then(() => {
          alert('Copied to clipboard!');
        })
        .catch(err => {
          console.error('Failed to copy text: ', err);
        });
    }
  }
};
</script> 