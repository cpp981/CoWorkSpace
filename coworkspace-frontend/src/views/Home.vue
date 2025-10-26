<template>
  <div class="home-container">
    <!-- Hero Section -->
    <header class="hero-section text-center py-5">
      <div class="container">
        <h1
          class="display-4 fw-bold mb-3 d-flex align-items-center justify-content-center gap-3 flex-column flex-sm-row"
        >
          <img
            src="../assets/logo1.png"
            alt="Logo"
            style="height: 80px; width: auto"
            class=""
          />
          <span>CoWorkSpace</span>
        </h1>
        <p class="lead text-muted mb-4">
          Reserva o publicita espacios de coworking modernos y conecta con
          profesionales en un entorno inspirador.
        </p>
        <div class="d-flex justify-content-center gap-3 flex-wrap">
          <button
            class="btn btn-outline-secondary"
            @click="$emit('open-login')"
          >
            <i class="bi bi-box-arrow-in-right me-2"></i>Iniciar Sesión
          </button>
          <button class="btn btn-primary" @click="$emit('open-register')">
            <i class="bi bi-person-plus me-2"></i>Registrarse
          </button>
        </div>
      </div>
    </header>

    <!-- Características -->
    <section class="features-section py-5">
      <div class="container">
        <h2 class="text-center fw-semibold mb-4">
          Qué puedes hacer en CoWorkSpace
        </h2>

        <div class="row g-3">
          <!-- Card 1: Anuncia tus espacios  -->
          <div class="col-12 col-md-6 col-lg-4">
            <div class="card h-100 shadow border-0">
              <div class="card-body d-flex flex-column">
                <div class="mb-3">
                  <i class="bi bi-megaphone fs-1"></i>
                </div>
                <h5 class="card-title text-primary">Anuncia tus espacios</h5>
                <p class="card-text text-muted flex-grow-1">
                  Publica salas, oficinas y zonas comunes con precios y
                  disponibilidad. Gestiona reservas y visibilidad desde tu
                  panel.
                </p>
                <div class="mt-3">
                  <button class="btn btn-outline-secondary btn-sm">
                    Saber más
                  </button>
                </div>
              </div>
            </div>
          </div>

          <!-- Card 2: Reserva tu espacio  -->
          <div class="col-12 col-md-6 col-lg-4">
            <div class="card h-100 shadow border-0">
              <div class="card-body d-flex flex-column">
                <div class="mb-3">
                  <i class="bi bi-calendar-check fs-1"></i>
                </div>
                <h5 class="card-title text-primary">Reserva tu espacio</h5>
                <p class="card-text text-muted flex-grow-1">
                  Busca por ubicación, precio o servicios. Reserva por horas,
                  días o meses y gestiona tus reservas en un solo lugar.
                </p>
                <div class="mt-3">
                  <button class="btn btn-outline-secondary btn-sm">
                    Buscar espacios
                  </button>
                </div>
              </div>
            </div>
          </div>

          <!-- Card 3: Designa administradores -->
          <div class="col-12 col-md-6 col-lg-4">
            <div class="card h-100 shadow border-0">
              <div class="card-body d-flex flex-column">
                <div class="mb-3">
                  <i class="bi bi-people-fill fs-1"></i>
                </div>
                <h5 class="card-title text-primary">Designa administradores</h5>
                <p class="card-text text-muted flex-grow-1">
                  Cede permisos a tu equipo para gestionar reservas y espacios.
                </p>
              </div>
            </div>
          </div>

          <!-- Card 4: Precios flexibles -->
          <div class="col-12 col-md-6 col-lg-4">
            <div class="card h-100 shadow border-0">
              <div class="card-body d-flex flex-column">
                <div class="mb-3">
                  <i class="bi bi-currency-dollar fs-1"></i>
                </div>
                <h5 class="card-title text-primary">Precios flexibles</h5>
                <p class="card-text text-muted flex-grow-1">
                  Define tus propias tarifas. Ofrece descuentos y promociones
                  temporales.
                </p>
              </div>
            </div>
          </div>

          <!-- Card 5: Dashboard de gestión -->
          <div class="col-12 col-md-6 col-lg-4">
            <div class="card h-100 shadow border-0">
              <div class="card-body d-flex flex-column">
                <div class="mb-3">
                  <i class="bi bi-bar-chart-line-fill fs-1"></i>
                </div>
                <h5 class="card-title text-primary">Dashboard de gestión</h5>
                <p class="card-text text-muted flex-grow-1">
                  Supervisa el rendimiento de tus espacios en un panel
                  intuitivo. Visualiza métricas clave y toma decisiones con
                  datos reales.
                </p>
              </div>
            </div>
          </div>

          <!-- Card 6: Opiniones y valoraciones -->
          <div class="col-12 col-md-6 col-lg-4">
            <div class="card h-100 shadow border-0">
              <div class="card-body d-flex flex-column">
                <div class="mb-3">
                  <i class="bi bi-star-fill fs-1"></i>
                </div>
                <h5 class="card-title text-primary">
                  Opiniones y valoraciones
                </h5>
                <p class="card-text text-muted flex-grow-1">
                  Recoge feedback de clientes y muestra puntuaciones para
                  aumentar confianza.
                </p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- Carrusel -->
    <section class="carousel-section py-5 bg-white">
      <div class="container">
        <h2 class="text-center fw-semibold mb-4">Anuncia tus Espacios</h2>
        <div
          id="coworkingCarousel"
          class="carousel slide"
          data-bs-ride="carousel"
        >
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
                style="height: 400px; object-fit: cover"
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

    <section class="cta-section py-5 bg-light">
      <div class="container text-center">
        <h3 class="fw-semibold mb-3">Únete a CoWorkSpace hoy</h3>
        <p class="text-muted mb-4">
          Forma parte de nuestra comunidad de profesionales y descubre los
          mejores espacios de coworking cerca de ti.
        </p>
        <button class="btn btn-primary btn-lg" @click="$emit('open-register')">
          Regístrate gratis
        </button>
      </div>
    </section>

    <!-- Footer -->
    <footer class="bg-dark text-white text-center py-4">
      <div class="container">
        <p class="mb-2">© 2025 CoWorkSpace</p>
        <p class="mb-0">
          <i class="bi bi-envelope me-2"></i>
          <a
            href="mailto:info@coworkspace.com"
            class="text-white text-decoration-underline"
          >
            info@coworkspace.com
          </a>
        </p>
      </div>
    </footer>
  </div>
</template>

<script>
export default {
  name: "HomePage",
  data() {
    return {
      images: [
        {
          src: "https://images.unsplash.com/photo-1517502884422-41eaead166d4?ixlib=rb-4.0.3&auto=format&fit=crop&w=1350&q=80",
          alt: "Espacio de coworking moderno",
        },
        {
          src: "https://images.unsplash.com/photo-1542744173-8e7e53415bb0?ixlib=rb-4.0.3&auto=format&fit=crop&w=1350&q=80",
          alt: "Oficina colaborativa",
        },
        {
          src: "https://images.unsplash.com/photo-1504384308090-c894fdcc538d?ixlib=rb-4.0.3&auto=format&fit=crop&w=1350&q=80",
          alt: "Equipo trabajando en coworking",
        },
      ],
    };
  },
  emits: ["open-login", "open-register"],
};
</script>
