# Zone 7 forensic validation

Fixture: `zone7_minimal_heating_from_climate_json`
Climate source: `EE.Doklad/Data/climate_zones.json`
Zone: `7` `7 - Sofia i Podbalkanskata dolina`
Heating season: `10-15` -> `04-23`
Oracle total QndPerArea: `30.88764` kWh/m2
EE.Doklad total QndPerArea: `30.88764` kWh/m2
Top suspected cause: `none detected`

| Month | Te_ClimateJson | Te_EeDokladProduction | DeltaTe | WorkDays_Oracle | WorkDays_EeDoklad | Saturdays_Oracle | Saturdays_EeDoklad | Sundays_Oracle | Sundays_EeDoklad | Holidays | ProjectHours_Oracle | ProjectHours_EeDoklad | NonProjectHours_Oracle | NonProjectHours_EeDoklad | Hve_Oracle | Hve_EeDoklad | Qve_Oracle | Qve_EeDoklad | DeltaQve | QndPerArea_Oracle | QndPerArea_EeDoklad | DeltaQndPerArea |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| October | 11.2 | 11.2 | 0 | 15 | 15 | 1 | 1 | 1 | 1 | 0 | 312 | 312 | 96 | 96 | 425 | 425 | 1536.1200000000003 | 1536.1200000000003 | 0 | 1.5361200000000004 | 1.5361200000000004 | 0 |
| November | 5.1 | 5.1 | 0 | 22 | 22 | 4 | 4 | 4 | 4 | 0 | 488 | 488 | 232 | 232 | 425 | 425 | 4471 | 4471 | 0 | 4.471 | 4.471 | 0 |
| December | 0.4 | 0.4 | 0 | 21 | 21 | 5 | 5 | 5 | 5 | 0 | 480 | 480 | 264 | 264 | 425 | 425 | 6064.920000000001 | 6064.920000000001 | 0 | 6.064920000000001 | 6.064920000000001 | 0 |
| January | -0.4 | -0.4 | 0 | 22 | 22 | 4 | 4 | 5 | 5 | 0 | 488 | 488 | 256 | 256 | 425 | 425 | 6331.479999999999 | 6331.479999999999 | 0 | 6.331479999999998 | 6.331479999999998 | 0 |
| February | 0.2 | 0.2 | 0 | 20 | 20 | 4 | 4 | 4 | 4 | 0 | 448 | 448 | 224 | 224 | 425 | 425 | 5559.68 | 5559.68 | 0 | 5.55968 | 5.55968 | 0 |
| March | 4.6 | 4.6 | 0 | 23 | 23 | 4 | 4 | 4 | 4 | 0 | 508 | 508 | 236 | 236 | 425 | 425 | 4784.479999999999 | 4784.479999999999 | 0 | 4.7844799999999985 | 4.7844799999999985 | 0 |
| April | 10.4 | 10.4 | 0 | 15 | 15 | 4 | 4 | 4 | 4 | 0 | 348 | 348 | 204 | 204 | 425 | 425 | 2139.96 | 2139.96 | 0 | 2.13996 | 2.13996 | 0 |
