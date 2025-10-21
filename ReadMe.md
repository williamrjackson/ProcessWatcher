# Process Enforcer Tray Utility

Background service that periodically checks for specified running processes. If a process isn't running (e.g., due to a crash), it will restart it.

Useful for unattended activities or kiosks.

---

## Basic Usage

### Launching the Application
- Run `ProcessEnforcerTray.exe` to start the application.
- Optionally, provide a path to a `.txt` file as a command-line argument to load a custom process list at startup.

### Adding Processes
- Click **Browse...** to select an executable (`.exe`) to add to the process list.
- You can edit arguments and delay for each process directly in the list view.

### Removing Processes
- Select one or more processes in the list and click **Remove** to delete them.

### Enforce Launch Order
- Toggle **Enforce Launch Order** to require processes to be launched in the specified order and with the specified delay.
- When this option is enabled, the application actively manages the running processes to guarantee the defined sequence. In this case the application may close running processes to restart them in the correct order.

### Saving and Loading Process Lists
- The process list is automatically saved to `processPaths.txt` in the application directory, or `%APPDATA%\Process Enforcer\processPaths.txt` if the default path is not writable.
- Use the **Load Launch File** menu option to load a different process list.

### UDP Communication
- The application listens for UDP messages on the configured address and port (default: `127.0.0.1:27000`).
- UDP messages can remotely update the process list. Send messages in the format:  
  `<exe_path>,<arguments>,<delay>,<exe_path>,<arguments>,<delay>,...`
    or a single ``.txt` filename to load a process list from a file.

### Configuring UDP Settings
- Use the **UDP Settings** menu option to change the listening address and port.

---
