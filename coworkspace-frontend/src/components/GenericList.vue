<template>
  <div>
    <!-- Título -->
    <h1 class="mb-4 text-primary">{{ title }}</h1>

    <!-- Barra de búsqueda -->
    <div class="input-group mb-3">
      <span class="input-group-text bg-white border-end-0 border-secondary">
        <i class="bi bi-search text-muted"></i>
      </span>
      <input
        v-model="search"
        type="text"
        class="form-control border-start-0 border-secondary"
        :placeholder="searchPlaceholder"
      />
    </div>

    <!-- Botón Agregar -->
    <div v-if="showAddButton" class="text-end mb-3">
      <button class="btn btn-outline-success" @click="$emit('add')">
        <i class="bi bi-plus-lg"></i> {{ addLabel }}
      </button>
    </div>

    <!-- Tabla -->
    <table class="table table-striped table-bordered">
      <thead class="table-light">
        <tr>
          <th v-for="header in headers" :key="header">{{ header }}</th>
          <th
            v-if="showActions || showManage"
            style="width: 120px"
            class="text-end"
          >
            Acciones
          </th>
        </tr>
      </thead>
      <tbody>
        <tr v-if="loading">
          <td
            :colspan="headers.length + (showActions ? 1 : 0)"
            class="text-center text-muted"
          >
            Cargando...
          </td>
        </tr>
        <tr v-else-if="filteredItems.length === 0">
          <td
            :colspan="headers.length + (showActions ? 1 : 0)"
            class="text-center text-muted"
          >
            {{ emptyMessage }}
          </td>
        </tr>
        <tr v-for="item in filteredItems" :key="item[idKey]">
          <template v-for="field in fields" :key="field">
            <!-- Si el nombre del campo termina en "Html" renderizamos HTML (v-html) -->
            <td v-if="field.endsWith('Html')" v-html="item[field]"></td>

            <!-- Si no, mostramos el valor normal  -->
            <td v-else>{{ item[field] }}</td>
          </template>
          <!-- Botones de editar y borrar -->
          <td v-if="showActions" class="text-end">
            <button
              class="btn btn-sm btn-warning me-2"
              @click="$emit('edit', item)"
            >
              <i class="bi bi-pencil"></i>
            </button>
            <button
              class="btn btn-sm btn-danger"
              @click="$emit('delete', item)"
            >
              <i class="bi bi-trash"></i>
            </button>
          </td>
          <!-- Botón de gestionar -->
          <td v-if="showManage" class="text-end">
            <button
              class="btn btn-sm btn-primary me-2"
              @click="$emit('manage', item)"
            >
              <i class="bi bi-gear"></i>
            </button>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script>
import { ref, computed } from "vue";

export default {
  name: "GenericList",
  props: {
    title: { type: String, required: true },
    items: { type: Array, required: true },
    headers: { type: Array, required: true },
    fields: { type: Array, required: true },
    idKey: { type: String, default: "id" },
    searchPlaceholder: { type: String, default: "Buscar..." },
    addLabel: { type: String, default: "Agregar" },
    emptyMessage: { type: String, default: "No se encontraron registros" },
    loading: { type: Boolean, default: false },
    showAddButton: { type: Boolean, default: true },
    showActions: { type: Boolean, default: true },
    showManage: { type: Boolean, default: true },
  },
  setup(props) {
    const search = ref("");

    const filteredItems = computed(() => {
      if (!search.value) return props.items;
      const q = search.value.toLowerCase();
      return props.items.filter((item) =>
        Object.values(item).some((val) => String(val).toLowerCase().includes(q))
      );
    });

    return { search, filteredItems };
  },
};
</script>
