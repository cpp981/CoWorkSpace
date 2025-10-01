<template>
    <nav class="mt-3 d-flex justify-content-center" v-if="totalPages > 1">
        <ul class="pagination">
            <li class="page-item" :class="{ disabled: currentPage === 1 }">
                <button class="page-link" @click="$emit('prev')">Anterior</button>
            </li>

            <li class="page-item" v-for="n in totalPages" :key="n" :class="{ active: currentPage === n }">
                <button class="page-link" @click="$emit('goToPage', n)">
                    {{ n }}
                </button>
            </li>

            <li class="page-item" :class="{ disabled: currentPage === totalPages }">
                <button class="page-link" @click="$emit('next')">Siguiente</button>
            </li>
        </ul>
    </nav>
</template>

<script setup>
defineProps({
    currentPage: { type: Number, required: true },
    totalPages: { type: Number, required: true },
});

defineEmits(["prev", "next", "goToPage"]);
</script>
