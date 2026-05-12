Repository: SerialCommunication — Copilot instructions

Build / test / lint commands
- No CI, test suite or lint scripts present.
- Build: open SerialCommunication.ino in the Arduino IDE and select the target board; or use arduino-cli. Example (specify your FQBN):
  arduino-cli compile --fqbn arduino:avr:uno C:\Users\annel\source\repos\SerialCommunication
- Visual Studio: open SerialCommunication.slnx (likely Visual Micro / VS Arduino solution) and use its build/run targets.
- Tests: none. Manual single-command test over serial: open serial monitor at 115200 and send `ping` (terminate with LF). Expect response `pong`.

High-level architecture
- Small Arduino sketch plus an embedded SerialCommand library:
  - SerialCommunication.ino: main sketch. Registers serial commands (set, toggle, get, ping, help, debug) via a SerialCommand instance and maps them to global handler functions.
  - SerialCommand.h / SerialCommand.cpp: lightweight command tokenizer/parser. Reads from a Stream, tokenizes on delim (default space), and calls registered handlers.
  - analog.c: included helper providing analogReadDelay(pin, microsec) for delayed ADC reads.
- Runtime behavior: setup() configures pin modes and command handlers, loop() repeatedly calls sCmd.readSerial() to process incoming serial lines. Commands use prefixes: dN (digital), aN (analog), pwmN (PWM outputs).

Key conventions & repository specifics
- Serial settings: Serial at 115200 bps (Baudrate macro).
- Command terminator: SerialCommand sets term='\n' in constructor — commands must be terminated by LF (line feed). Some SerialCommand comments reference '\r' but the code uses '\n'.
- Command/token limits: SERIALCOMMANDBUFFER = 32 limits command and argument length; MAXSERIALCOMMANDS = 10 limits number of registered commands.
- Command naming:
  - Digital pins: use prefix "d" (e.g., d2..d7). For outputs controlled by set/toggle, valid d2..d4.
  - PWM pins: use prefix "pwm" (e.g., pwm9..pwm11), values 0..255.
  - Analog inputs: use prefix "a" (a0..a5). analogReadDelay is used with a 50000 microsecond sample delay in get handler.
- Memory/flash strings: code uses F("...") to store constant strings in flash to reduce RAM usage.
- Debugging: SERIALCOMMANDDEBUG macro exists in SerialCommand.h but is #undef'ed by default. Uncomment to enable verbose command-parse echoing.
- Handler signatures: addCommand expects void(*)() handlers; default handler signature is void (*)(char*). Command handler functions use sCmd.next() to read arguments.

Files & AI-config reconnaissance
- No README/CONTRIBUTING/CLAUDE/AGENTS/AIDER/CURSOR/WINDSURF specific AI config files detected in repository root.

Notes for Copilot sessions
- Inspect SerialCommand.h/.cpp first to understand tokenization, buffer sizes and how handlers are invoked.
- Check SerialCommunication.ino for command semantics, pin ranges and argument validation (isValidNumber, startsWith).
- Be conservative when changing buffer/command limits (SERIALCOMMANDBUFFER / MAXSERIALCOMMANDS) — hardware constraints and memory usage are tight on AVR devices.

If any of these commands or board targets should be added to repo-level scripts (CI, arduino-cli.json, or a Makefile), state desired boards and test harness and Copilot can add them.
