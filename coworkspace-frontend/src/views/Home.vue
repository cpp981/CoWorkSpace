<template>
  <div class="home-container">
    <!-- Hero Section -->
    <header class="hero-section text-center py-5">
      <div class="container">
        <h1 class="display-4 fw-bold mb-3">
          <i class="bi bi-building me-2"></i>CoWorkSpace
        </h1>
        <p class="lead text-muted mb-4">
          Reserva espacios de coworking modernos y conecta con profesionales en un entorno inspirador.
        </p>
        <div class="d-flex justify-content-center gap-3">
          <button
            class="btn btn-primary"
            @click="$emit('open-login')"
          >
            <i class="bi bi-box-arrow-in-right me-2"></i>Iniciar Sesión
          </button>
          <button
            class="btn btn-outline-secondary"
            @click="$emit('open-register')"
          >
            <i class="bi bi-person-plus me-2"></i>Registrarse
          </button>
        </div>
      </div>
    </header>

    <!-- Carrusel -->
    <section class="carousel-section py-5 bg-white">
      <div class="container">
        <h2 class="text-center fw-semibold mb-4">Descubre Nuestros Espacios</h2>
        <div id="coworkingCarousel" class="carousel slide" data-bs-ride="carousel">
          <div class="carousel-inner">
            <div
              v-for="(image, index) in images"
              :key="index"
              :class="['carousel-item', { active: index === 0 }]"
            >
              <img
                :src="image.src"
                :alt="image.alt"
                class="d-block w-100 rounded shadow"
                style="height: 400px; object-fit: cover;"
              />
            </div>
          </div>
          <button
            class="carousel-control-prev"
            type="button"
            data-bs-target="#coworkingCarousel"
            data-bs-slide="prev"
          >
            <span class="carousel-control-prev-icon" aria-hidden="true"></span>
            <span class="visually-hidden">Previous</span>
          </button>
          <button
            class="carousel-control-next"
            type="button"
            data-bs-target="#coworkingCarousel"
            data-bs-slide="next"
          >
            <span class="carousel-control-next-icon" aria-hidden="true"></span>
            <span class="visually-hidden">Next</span>
          </button>
        </div>
      </div>
    </section>

    <!-- Footer -->
    <footer class="bg-dark text-white text-center py-4">
      <div class="container">
        <p class="mb-2">
          <i class="bi bi-geo-alt me-2"></i>© 2025 CoWorkSpace. Todos los derechos reservados.
        </p>
        <p class="mb-0">
          <i class="bi bi-envelope me-2"></i>
          <a href="mailto:info@coworkspace.com" class="text-white text-decoration-underline">
            info@coworkspace.com
          </a>
        </p>
      </div>
    </footer>
  </div>
</template>

<script>
export default {
  name: 'HomePage',
  data() {
    return {
      images: [
        {
          src: 'https://images.unsplash.com/photo-1517502884422-41eaead166d4?ixlib=rb-4.0.3&auto=format&fit=crop&w=1350&q=80',
          alt: 'Espacio de coworking moderno',
        },
        {
          src: 'https://images.unsplash.com/photo-1542744173-8e7e53415bb0?ixlib=rb-4.0.3&auto=format&fit=crop&w=1350&q=80',
          alt: 'Oficina colaborativa',
        },
        {
          src: 'https://images.unsplash.com/photo-1504384308090-c894fdcc538d?ixlib=rb-4.0.3&auto=format&fit=crop&w=1350&q=80',
          alt: 'Equipo trabajando en coworking',
        },
      ],
    };
  },
  emits: ['open-login', 'open-register'],
};
</script>