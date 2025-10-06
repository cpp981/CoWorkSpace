<template>
    <div class="generic-calendar">
        <FullCalendar :options="calendarOptions" class="calendar" />
    </div>
</template>

<script setup>
import { computed } from "vue";
import FullCalendar from "@fullcalendar/vue3";
import dayGridPlugin from "@fullcalendar/daygrid";
import timeGridPlugin from "@fullcalendar/timegrid";
import interactionPlugin from "@fullcalendar/interaction";
import esLocale from "@fullcalendar/core/locales/es"; // Locale español

const props = defineProps({
    events: {
        type: Array,
        default: () => [],
    },
    initialView: {
        type: String,
        default: "timeGridWeek",
    },
    editable: {
        type: Boolean,
        default: false,
    },
});

const emits = defineEmits(["event-click", "date-click"]);

const calendarOptions = computed(() => ({
    plugins: [dayGridPlugin, timeGridPlugin, interactionPlugin],
    initialView: props.initialView,
    events: props.events,
    editable: props.editable,
    locale: esLocale,
    headerToolbar: {
        left: "prev,next today",
        center: "title",
        right: "dayGridMonth,timeGridWeek,timeGridDay",
    },
    eventClick(info) {
        emits("event-click", info.event);
    },
    dateClick(info) {
        emits("date-click", info.date);
    },
}));
</script>

<style scoped>
.generic-calendar {
    width: 100%;
    overflow-x: auto;
}

.calendar {
    max-width: 100%;
}
</style>
