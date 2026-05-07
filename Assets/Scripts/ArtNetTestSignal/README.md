# Art-Net Test Signal Tool Base

This sandbox tool is a UI-first base for generating Art-Net/DMX test signals before choosing the final networking library.

## Concept

- `ArtNetTestSignalWindow` is the Editor UI available from `VLiveKit > Tools > Art-Net Test Signal`.
- `ArtNetSignalGenerator` creates DMX byte frames without depending on any network package.
- `ArtNetSenderConfig` stores target IP, port, Net/Subnet/Universe, and refresh rate.
- `IArtNetPacketSender` is the seam for the future Art-Net library integration.
- `NullArtNetPacketSender` keeps the tool usable now by generating and previewing frames without sending network packets.

## Next integration step

When the Art-Net library is selected, implement `IArtNetPacketSender` with that library and replace the `NullArtNetPacketSender` instance in the Editor window. The UI and signal generator should not need to change unless the selected library requires additional connection settings.
