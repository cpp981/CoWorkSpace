<template>
    <div class="card h-100 shadow-sm bg-secondary bg-opacity-25">
        <div class="card-body d-flex flex-column justify-content-between">
            <div class="d-flex justify-content-between align-items-start mb-2">
                <h5 class="card-title text-primary mb-0">{{ space.nombre }}</h5>
                <span class="badge rounded-pill d-flex align-items-center gap-1"
                    :class="space.estado === 'Libre' ? 'bg-success-subtle text-success' : 'bg-danger-subtle text-danger'">
                    <span class="status-dot rounded-circle"
                        :class="space.estado === 'Libre' ? 'bg-success' : 'bg-danger'"></span>
                    {{ space.estado }}
                </span>
            </div>

            <ul class="list-unstyled small text-muted mb-3">
                <li><strong>Ciudad:</strong> {{ space.ciudad }}</li>
                <li><strong>Precio:</strong> {{ Number(space.precio).toFixed(2) }} €</li>
                <li><strong>Siguiente Reserva:</strong> {{ space.siguienteReserva && space.siguienteReserva.trim() !==
                    ''
                    ? formatDate(space.siguienteReserva)
                    : 'Sin próximas reservas' }}</li>
            </ul>

            <!-- Botones de acción -->
            <div class="d-flex gap-2 mt-2">
                <button class="btn btn-sm btn-outline-primary" @click="$emit('view-calendar', space)">
                    <i class="bi bi-calendar-range"></i> Ver Calendario
                </button>
                <button class="btn btn-sm btn-outline-dark" @click="$emit('view-management', space)">
                    <i class="bi bi-gear"></i> Gestionar Reservas
                </button>
            </div>
        </div>
    </div>
</template>

<script setup>
defineProps({
    space: {
        type: Object,
        required: true
    }
});
defineEmits(["view-calendar"]);

// Función para formatear la fecha
const formatDate = (dateStr) => {
    if (!dateStr || dateStr.trim() === "") return "";
    const date = new Date(dateStr);
    const day = String(date.getDate()).padStart(2, "0");
    const month = String(date.getMonth() + 1).padStart(2, "0"); // Enero = 0
    const year = date.getFullYear();
    const hours = String(date.getHours()).padStart(2, "0");
    const minutes = String(date.getMinutes()).padStart(2, "0");
    return `${day}-${month}-${year} - ${hours}:${minutes} h`;
};
</script>