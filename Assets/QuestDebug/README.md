# Quest Debug Log

## About
A primitive world-space log console for monitoring your custom debugging statements while testing in your Oculus build.


## Usage

### Scripting
QuestDebug follows a singleton pattern. On awake, a singleton instance is created as a static property of the class. This static instance can be accessed like so:
`QuestDebug.Instance`

To log a message to the console, use the singleton's `Log()` method. For example:
`QuestDebug.Instance.Log("This is a test...");`

### In-Game
Toggle the console's visibility by pressing the B button (`Button.Two`) on your right Touch controller.