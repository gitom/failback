# Failback

Failback is a .Net library that allows you to cache method call results, and return these  in case of 
a failure or unavailability; either planned or unplanned.

One can monitor and activate/de-activate these at run-time, using a feature toggle platform.

## Goal

The goal of this project is two-fold:

- Explore options regarding the usability of run-time toggles like these
- Get more familiar with existing libraries & platforms:
  - [Polly]() or [Hystrix]()
  - [LaunchDarkly]()
  - [Brightr]() or [Mediatr]()
