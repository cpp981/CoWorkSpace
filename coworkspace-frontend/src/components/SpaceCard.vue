<template>
  <div class="card h-100 shadow-sm bg-secondary bg-opacity-25">
    <div class="card-body d-flex flex-column justify-content-between">
      <div class="d-flex justify-content-between align-items-start mb-2">
        <h5 class="card-title mb-0">{{ space.name }}</h5>
        <span class="badge rounded-pill"
          :class="space.isActive ? 'bg-success-subtle text-success' : 'bg-danger-subtle text-danger'">
          {{ space.isActive ? 'Activo' : 'Inactivo' }}
        </span>
      </div>

      <ul class="list-unstyled small text-muted mb-3">
        <li><strong>Ciudad:</strong> {{ space.city }}</li>
        <li><strong>Precio:</strong> {{ space.price.toFixed(2) }} €</li>
        <li>
          <strong>Tipo:</strong>
          <span
            :class="space.isPublic ? 'badge badge-transparent-success ms-2' : 'badge badge-transparent-danger ms-2'">
            {{ space.isPublic ? 'Público' : 'Privado' }}
          </span>
        </li>
      </ul>

      <!-- Botón que emite el evento -->
      <div class="d-flex gap-2 mt-2">
        <button class="btn btn-sm btn-outline-primary" @click="$emit('view-bookings', space)"><i
            class="bi bi-journal-text"></i>Ver Reservas</button>
        <button class="btn btn-sm btn-outline-warning" @click="$emit('edit-space', space)"><i
            class="bi bi-pencil"></i>Editar</button>
        <button class="btn btn-sm btn-outline-danger" @click="$emit('delete-space', space)"><i
            class="bi bi-trash"></i>Borrar</button>
      </div>
    </div>
  </div>
</template>

<script setup>
defineProps({
  space: Object
});
defineEmits(["view-bookings", "edit-space", "delete-space"]);
</script>
