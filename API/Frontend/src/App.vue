<template>
  <div class="app-container">
    <nav class="navbar navbar-expand-lg navbar-dark bg-primary mb-4">
      <div class="container">
        <router-link class="navbar-brand" to="/">
          <i class="bi bi-link-45deg"></i> URL Shortener
        </router-link>
      </div>
    </nav>

    <main class="container mb-5">
      <router-view></router-view>
    </main>

    <footer class="footer mt-auto py-3 bg-light">
      <div class="container text-center">
        <span class="text-muted">URL Shortener &copy; {{ new Date().getFullYear() }} - AMD201 Coursework</span>
      </div>
    </footer>
  </div>
</template>

<script>
export default {
  name: 'App'
}
</script>

<style>
.app-container {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}

main {
  flex-grow: 1;
}

.footer {
  margin-top: auto;
}
</style> 