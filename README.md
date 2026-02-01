DCProductionPlant – Digital Twin (Unity)
A Unity-based digital twin prototype of a DC (Direct Current) production plant, intended as a visual, interactive representation of key production assets and flows. The project serves as a foundation for further work on monitoring, visualization, and XR integration around DC production systems.
​

Features
Unity project structure with Assets, Packages, ProjectSettings, and custom UIAssets for UI elements.
​

Ready-to-extend scene setup for modeling DC production equipment, layout, and process logic.

Basis for integrating real-time data (e.g., OPC UA, MQTT, REST) and analytics into a 3D plant model.

Designed to be compatible with common Unity workflows for desktop and XR targets (e.g., VR headsets) once corresponding SDKs are added.

Project Structure
Assets/ – Main Unity content (scenes, scripts, prefabs, materials, etc.).
​

UIAssets/ – UI graphics and resources specific to the plant dashboard and in-scene interfaces.
​

Packages/ – Unity package manifest and dependencies managed by the Unity Package Manager.
​

ProjectSettings/ – Unity project and editor configuration.
​

.vscode/ – Editor configuration for VS Code (recommended for C# development).[
​

.utmp/ – Unity-related temporary or tooling metadata.
​

.gitignore – VCS ignore rules tailored for Unity.
​

Getting Started
Prerequisites
Unity (recommended 2021 LTS or newer; use the version closest to the one configured when opening the project).

.NET / C# tooling (installed automatically with Unity, or via Visual Studio / VS Code).

Git (optional, for version control and collaboration).

Setup
Clone the repository:

bash
git clone https://github.com/Abhilash1205/DCProductionPlant-Digital-Twin.git
cd DCProductionPlant-Digital-Twin
Open Unity Hub, click “Open project”, and select this folder.

Let Unity import all assets and packages.

Open the main scene inside Assets/ (for example, Main.unity or a plant-related scene name once created).

Press Play in the Unity Editor to run the current simulation.

Usage Ideas
Visualize the DC production line layout (machines, conveyors, buffers, AGVs, etc.).

Attach scripts to simulate material flow, machine states, and KPIs (throughput, OEE, energy usage).

Integrate with live shop-floor data to turn the scene into an operational digital twin dashboard.

Extend with VR/AR/XR support using Unity XR plugins for immersive monitoring and control.

Roadmap
Add core plant layout and equipment models.

Implement data interface layer (e.g., OPC UA/MQTT bridge).

Develop operator UI panels using UIAssets.

Introduce XR support (VR/AR) for immersive plant inspection and training.

Document scene hierarchy and scripting conventions.

Contributing
Contributions are welcome:

Fork the repository.

Create a feature branch:

bash
git checkout -b feature/my-improvement
Commit and push your changes.

Open a pull request describing your changes and testing steps.

License
Specify a license here (for example, MIT, Apache 2.0, or a custom license). If no explicit license is added, the repository is not automatically open for reuse—consider adding a LICENSE file.
