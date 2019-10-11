<template>
  <div class="home">
    <img alt="Vue logo" src="../assets/logo.png">
    <h1>Welcome to Your Vue.js App</h1>
     <table>
      <thead>
        <tr>
          <th v-for="(item, index) in forecastCols" v-bind:key="index"> 
            {{ item.label | capitalize }}
          </th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="(item, index) in forecasts" v-bind:key="index">
          <td v-for="(col, index) in forecastCols" v-bind:key="index">
            {{ col.field(item) }}
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script lang="ts">
import { Component, Vue} from 'vue-property-decorator';
import { IWeatherForecast } from '../models/IWeatherForecast';
import axios from 'axios';

@Component
export default class Home extends Vue {
  private forecasts: IWeatherForecast[] = [{ summary: 'No data.' } as IWeatherForecast];
  private forecastCols: any[] = [
    { name: 'Summary', label: 'Summary', field: (row: IWeatherForecast) => row.summary },
    { name: 'F',       label: 'F',       field: (row: IWeatherForecast) => row.temperatureF },
    { name: 'C',       label: 'C',       field: (row: IWeatherForecast) => row.temperatureC },
    { name: 'Date',     label: 'Date',   field: (row: IWeatherForecast) => row.date },
  ];

  public async mounted() {
    try {
      this.forecasts = (await axios.get('api/weatherforecast')).data;
    } catch {
      this.forecasts = [{ summary: 'No data.' } as IWeatherForecast];
    }
  }
}
</script>