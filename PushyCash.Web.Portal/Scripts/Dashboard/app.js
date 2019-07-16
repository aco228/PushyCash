Vue.component('v-select', VueSelect.VueSelect);
const API_URL = "http://callback.pushycash.mobilepaywall.com/api";

new Vue({
  el: '#app',
  data: {
    countries: [],
    mobileOperators: [],
    trackingOptions: [],
    verticals: [
      { VerticalID: 1, Name: 'adult' },
      { VerticalID: 2, Name: 'mainstream' },
      { VerticalID: 3, Name: 'aggressive' }
    ],
    devices: [
      { DeviceID: 1, Name: 'Android' },
      { DeviceID: 2, Name: 'iOS' }
    ],
    selectedVertical: null,
    selectedCountry: null,
    selectedMobileOperator: null,
    selectedDevice: null
  },
  methods: {
    async onSelect() {

      let country = this.selectedCountry != null ? this.selectedCountry.Name : "";
      let mobileOperator = this.selectedMobileOperator != null ? this.selectedMobileOperator.Name : "";
      let device = this.selectedDevice != null ? this.selectedDevice.Name : "";
      let vertical = this.selectedVertical != null ? this.selectedVertical.Name : "";

      const url = API_URL + `/trackingoptions?country=${country}&mobileoperator=${mobileOperator}&vertical=${vertical}&device=${device}`;

      const response = await axios(url);
      this.trackingOptions = response.data.map(tp => {
        let t = { ...tp, edit: false, new: false};
        return t;
      });
    },
    onEdit(id) {
      let trackingOption = this.trackingOptions.find(tr => tr.TrackingOptionsID == id);
      trackingOption.edit = true;
    },
    async onUpdate(id) {
      let trackingOption = this.trackingOptions.find(tr => tr.TrackingOptionsID == id);

      const conversions = trackingOption.TrackingConversions;
      const payout = trackingOption.TrackingPayout;
      const minutes = trackingOption.TrackingMinutes;
      const cpc = trackingOption.ClicksPerConversion;
      const td = trackingOption.ToleratedDeficit;

      if (trackingOption.TrackingOptionsID == 1) {
        if (conversions == "" || payout == "" || minutes == "" || cpc == "" || td == "") {
          await swal("Error!", "All editable data for default to must be entered!", "error");
          return;
        }
      }

      await axios.put(API_URL + "/trackingoptions/" + id,
              {
                TrackingConversions: conversions,
                TrackingPayout: payout,
                TrackingMinutes: minutes,
                ClicksPerConversion: cpc,
                ToleratedDeficit: td
        });

      swal("Success!", "Tracking offer has been updated!", "success");
      trackingOption.edit = false;
    },
    onAdd() {
      this.trackingOptions.unshift({
        TrackingOptionsID: -1,
        CountryName: null,
        Vertical: null,
        Device: null,
        MobileOperator: null,
        TrackingConversions: "",
        TrackingPayout: "",
        TrackingMinutes: "",
        ClicksPerConversion: "",
        ToleratedDeficit: "",
        edit: true,
        new: true
      });
    },
    async add(trackingOptions) {
      
      const response = await axios.post(API_URL + "/trackingoptions", trackingOptions);

      swal("Success!", "Tracking offer has been updated!", "success");

      trackingOptions.TrackingOptionsID = response.data;
      trackingOptions.new = false;
      trackingOptions.edit = false;
    },
    async onDelete(id) {

      if (id == 1) { return;}

      const shouldDelete = await swal({
        title: "Are you sure?",
        icon: "warning",
        buttons: true,
        dangerMode: true,
      });

      if (shouldDelete) {

        await axios.delete(API_URL + "/trackingoptions/" + id);
        
        const tid = this.trackingOptions.findIndex(t => t.TrackingOptionsID == id);
        this.trackingOptions.splice(tid, 1);

        swal("Deleted!", "TrackingOption has been deleted", "success");
      }

    

    }
    
  },
  watch: {
    selectedCountry() {
      let countryID = this.selectedCountry != null ? this.selectedCountry.CountryID : -1;
      this.mobileOperators = MOBILE_OPERATORS.filter(m => m.CountryID == countryID);
    }
  },
  created() {
    this.countries = COUNTRIES;
  }
});