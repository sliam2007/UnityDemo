# Industrial Training Simulator

A small Unity-based technical training simulator demonstrating real-time machine logic, operator controls, telemetry, fault handling, and recovery procedures.

## Features

- Machine state flow: `Off`, `Ready`, `Running`, `Fault`, `EmergencyStop`
- Simulated temperature, pressure, and motor speed
- Gradual motor acceleration and deceleration
- Cooling failure and overheat scenario
- Emergency stop with immediate motor shutdown
- Safety interlocks for operator controls
- Live telemetry and operator messages
- 3D rotor visualization driven by simulated RPM
- Cooling fan that runs with the machine and coasts down after a normal stop
- Spatial motor and cooling-fan sounds that follow their respective speeds
- Enclosed, collidable room with a free-fly camera that slides along surfaces
- Six glowing ceiling fixtures with soft, oval light patches and one intermittently flickering fixture

## Controls

- **POWER ON** — switches the machine to `Ready`
- **START** — starts the machine
- **STOP** — performs a normal stop
- **POWER OFF** — switches off a stopped machine
- **EMERGENCY STOP** — immediately stops the motor
- **COOLING FAILURE** — activates the cooling failure scenario
- **RESTORE COOLING** — restores cooling after an overheat fault
- **RESET** — resets the machine after safe conditions are restored
- **EXIT** — closes the standalone application

### Camera Controls

- Hold **Right Mouse Button** and move the mouse — look around
- Hold **Right Mouse Button** and use **WASD** — move the camera
- Hold **Right Mouse Button** and use **Q / E** — move down or up
- Hold **Shift** while moving — increase movement speed

The camera cannot pass through the room or machine colliders; it slides along surfaces when blocked.

## Safety Logic

A reset after a fault is permitted only when:

- cooling has been restored;
- temperature is below 60 °C;
- motor speed is 0 RPM.

If the machine is stopped before the overheat threshold is reached, the pending overheat scenario is cancelled.

## Architecture

The project separates simulation logic from Unity presentation components:

- `DeviceStateMachine` — machine state transitions
- `SensorModel` — temperature, pressure, and motor speed simulation
- `FaultManager` — cooling failure and reset conditions
- `SimulationController` — coordinates the simulation
- `OperatorPanelView` — button availability and operator controls
- `TelemetryView` — telemetry and status display
- `MotorRotorView` — visual rotor movement
- `MachineStatusLightView` — visual machine status indicator
- `CoolingFanView` — cooling-fan rotation and coast-down
- `MotorSoundView` and `CoolingFanSoundView` — spatial machine audio
- `FlyCameraController` — camera movement and collision
- `FaultyCeilingLightView` and `CeilingLightCookieView` — intermittent flicker and oval ceiling-light masks

## Requirements

- Unity 6.3 LTS (`6000.3.24f1`)
- Universal Render Pipeline
- Windows build target

## Running the Project

1. Open the project in Unity.
2. Open `Assets/Scenes/Simulator.unity`.
3. Enter Play Mode.

A standalone Windows build can be created from **File → Build Profiles → Windows**.
