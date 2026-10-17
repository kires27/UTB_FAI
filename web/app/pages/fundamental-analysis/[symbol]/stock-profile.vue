<script setup>
import { cached } from "~/utils/cache"
import { API } from "#shared/constants/routes"

useHead({ title: "MEIC — Company Profile" })

const route = useRoute()
const symbol = computed(() =>
	(route.params.symbol || "").toString().toUpperCase(),
)

const stockData = ref(null)

watch(symbol, async () => {
	stockData.value = null

	stockData.value = await cached(
		`fa-profile-${symbol.value}`,
		() => $fetch(API.stocks.get_stock(symbol.value)),
	)
}, { immediate: true })

const companyInfo = computed(() => [
	{ label: "Full Name", value: stockData.value?.name },
	{ label: "Founded", value: "2003" },
	{ label: "Headquarters", value: stockData.value?.address + ", " + stockData.value?.city },
	{ label: "Employees", value: stockData.value?.employees },
	{ label: "Sector", value: stockData.value?.sector },
	{ label: "Industry", value: stockData.value?.industry },
	{ label: "Exchange", value: stockData.value?.exchange },
	{ label: "Currency", value: stockData.value?.currency },
	{ label: "Fiscal Year End", value: "December" },
	{ label: "Website", value: stockData.value?.website, link: true },
])

const description = computed(() => [stockData.value?.description])

const leadership = computed(() => stockData.value?.key_executives ?? [])

const revenueByProduct = [
	{ name: "Automotive", value: 85, color: "var(--teal)" },
	{ name: "Energy", value: 12, color: "var(--blue)" },
	{ name: "Services", value: 3, color: "var(--purple)" },
]

const markets = [
	{ name: "United States", value: 53 },
	{ name: "China", value: 30 },
	{ name: "Europe", value: 11 },
	{ name: "Rest of World", value: 6 },
]

const marketColors = [
	"var(--teal)",
	"var(--gold)",
	"var(--purple)",
	"var(--blue)",
]

const facilities = [
	{ location: "Fremont, CA", products: "Model S, X, 3, Y" },
	{
		location: "Shanghai, China",
		products: "Model 3, Y (export)",
	},
	{ location: "Berlin, Germany", products: "Model Y (EU)" },
	{ location: "Austin, TX", products: "Cybertruck, Model Y" },
]

</script>

<template>
	<Container>
		<div v-if="stockData" class="profile-grid">

			<!-- Company Info -->
			<div class="cell">
				<div class="cell-header">
					<Text variant="label">Company Info</Text>
				</div>
				<table class="info-table">
					<tbody>
						<tr v-for="(item, idx) in companyInfo" :key="idx">
							<td><Text variant="body-sm" tone="muted">{{ item.label }}</Text></td>
							<td>
								<a v-if="item.link" :href="item.value" target="_blank" rel="noopener noreferrer">
									<Text variant="body-sm" tone="green" link>{{ item.value }} ↗</Text>
								</a>
								<Text v-else variant="body-sm">{{ item.value }}</Text>
							</td>
						</tr>
					</tbody>
				</table>
			</div>

			<!-- <img :src="priceGraph" alt="" class="description-cell"> -->

			<!-- Description -->
			<div class="cell description-cell">
				<div class="cell-header">
					<Text variant="label">Description</Text>
				</div>
				<Text v-for="(text, idx) in description" :key="idx" variant="body-sm" tone="muted" class="desc-text">
					{{ text }}
				</Text>
			</div>

			<!-- Leadership -->
			<div class="cell">
				<div class="cell-header">
					<Text variant="label">Leadership</Text>
				</div>
				<div v-for="person in leadership" :key="person.name" class="person-row">
					<div class="person-avatar"><Text variant="body-sm" tone="muted">{{ person.initials }}</Text></div>
					<div class="person-info">
						<Text variant="body-sm" weight="medium">{{ person.name }}</Text>
						<Text variant="body-sm" tone="muted">{{ person.title }}</Text>
					</div>
					<Text v-if="person.age" variant="body-sm" tone="muted" class="person-since">age: {{ person.age
					}}</Text>
				</div>
			</div>


			



			<!-- Revenue Breakdown -->
			<div class="cell">
				<div class="cell-header">
					<Text variant="label">Revenue Breakdown By Product</Text>
				</div>
				<div class="donut-row">
					<div class="donut-wrap">
						<svg viewBox="0 0 100 100" width="88" height="88">
							<circle cx="50" cy="50" r="40" fill="none" stroke="var(--bg-hover)" stroke-width="14" />
							<circle cx="50" cy="50" r="40" fill="none" stroke="var(--teal)" stroke-width="14"
								stroke-dasharray="213 38" stroke-dashoffset="0" opacity="0.85"
								transform="rotate(-90 50 50)" />
							<circle cx="50" cy="50" r="40" fill="none" stroke="var(--blue)" stroke-width="14"
								stroke-dasharray="30 221" stroke-dashoffset="-213" opacity="0.75"
								transform="rotate(-90 50 50)" />
							<circle cx="50" cy="50" r="40" fill="none" stroke="var(--purple)" stroke-width="14"
								stroke-dasharray="8 243" stroke-dashoffset="-243" opacity="0.75"
								transform="rotate(-90 50 50)" />
							<text x="50" y="54" text-anchor="middle" fill="var(--fg)" font-family="var(--font-main)"
								font-size="11" font-weight="500">85%</text>
						</svg>
						<div class="donut-legend">
							<div v-for="item in revenueByProduct" :key="item.name" class="legend-item">
								<div class="legend-dot" :style="{ background: item.color }"></div>
								<Text variant="body-sm" tone="muted">{{ item.name }}</Text>
								<Text variant="body-sm" tone="muted" class="legend-pct">{{ item.value }}%</Text>
							</div>
						</div>
					</div>
				</div>
			</div>



			<!-- Markets + Facilities -->
			<div class="cell">
				<div class="cell-header">
					<Text variant="label">Geographic Markets</Text>
				</div>
				<div class="market-list">
					<div v-for="(market, idx) in markets" :key="market.name" class="market-item">
						<div class="market-header">
							<Text variant="body-sm" tone="muted">{{ market.name }}</Text>
							<Text variant="body-sm" weight="medium">{{ market.value }}%</Text>
						</div>
						<div class="track">
							<div class="track-fill" :style="{
								width: market.value + '%',
								background: marketColors[idx],
							}"></div>
						</div>
					</div>
				</div>

				<div class="cell-header" style="margin-top: 1.5rem">
					<Text variant="label">Production Facilities</Text>
				</div>
				<table class="info-table">
					<tbody>
						<tr v-for="fac in facilities" :key="fac.location">
							<td><Text variant="body-sm" tone="muted">{{ fac.location }}</Text></td>
							<td><Text variant="body-sm">{{ fac.products }}</Text></td>
						</tr>
					</tbody>
				</table>
			</div>
		</div>
	</Container>
</template>

<style scoped>
/* ── Grid ── */
.profile-grid {
	display: grid;
	grid-template-columns: 1fr 1fr 1fr;
	border-left: 1px solid var(--border);
	border-top: 1px solid var(--border);
	margin-bottom: 2rem;
}

.profile-grid>* {
	border-right: 1px solid var(--border);
	border-bottom: 1px solid var(--border);
}

/* ── Cell ── */
.cell {
	padding: 1.5rem;
}

/* ── Description spans 2 columns ── */
.description-cell {
	grid-column: span 2;
}

/* ── Cell header — shared pattern ── */
.cell-header {
	display: flex;
	align-items: center;
	justify-content: space-between;
	padding-bottom: 0.6rem;
	border-bottom: 1px solid var(--border);
	margin-bottom: 1rem;
}

/* ── Info table ── */
.info-table {
	width: 100%;
	border-collapse: collapse;
}

.info-table tr {
	border-bottom: 1px solid var(--border);
	transition: background 0.1s;
}

.info-table tr:last-child {
	border-bottom: none;
}

.info-table tr:hover {
	background: rgba(255, 255, 255, 0.02);
}

.info-table td {
	padding: 0.5rem 0;
	vertical-align: top;
}

.info-table td:first-child {
	color: var(--fg-3);
	width: 45%;
}

.info-table td:last-child {
	text-align: right;
}

/* ── Description ── */
.desc-text {
	margin-bottom: 1rem;
}

.desc-text:last-of-type {
	margin-bottom: 0;
}

/* ── Revenue donut ── */
.donut-row {
	display: flex;
	gap: 1.5rem;
}

.donut-wrap {
	flex: 1;
	display: flex;
	flex-direction: column;
	align-items: center;
	gap: 0.6rem;
}

.donut-legend {
	display: flex;
	flex-direction: column;
	gap: 0.3rem;
	width: 100%;
}

.legend-item {
	display: flex;
	align-items: center;
	gap: 0.5rem;
}

.legend-dot {
	width: 6px;
	height: 6px;
	border-radius: 50%;
	flex-shrink: 0;
}

.legend-pct {
	margin-left: auto;
}

/* ── Leadership ── */
.person-row {
	display: flex;
	align-items: center;
	gap: 0.75rem;
	padding: 0.6rem 0;
	border-bottom: 1px solid var(--border);
	transition: background 0.1s;
}

.person-row:last-child {
	border-bottom: none;
}

.person-row:hover {
	background: rgba(255, 255, 255, 0.02);
}

.person-avatar {
	width: 30px;
	height: 30px;
	background: var(--bg-hover);
	border: 1px solid var(--border-hi);
	display: flex;
	align-items: center;
	justify-content: center;
	flex-shrink: 0;
}

.person-info {
	flex: 1;
}

/* ── Markets ── */
.market-list {
	display: flex;
	flex-direction: column;
	gap: 0.65rem;
}

.market-header {
	display: flex;
	justify-content: space-between;
	margin-bottom: 0.3rem;
}

/* ── Track bar — unified ── */
.track {
	height: 3px;
	background: var(--bg-hover);
	overflow: hidden;
}

.track-fill {
	height: 100%;
	transition: width 0.4s ease;
}

/* ── Responsive ── */
@media (max-width: 900px) {
	.profile-grid {
		grid-template-columns: 1fr;
	}

	.donut-row {
		flex-direction: column;
		align-items: flex-start;
	}
}
</style>
