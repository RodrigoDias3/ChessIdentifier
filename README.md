# ChessIdentifier

This project is a Chess Board Identifier that processes a given image of a chessboard, detects the board and pieces, and provides statistics for both White and Black teams. The program identifies the position of each piece and outputs their counts and locations.

![imagem](https://github.com/user-attachments/assets/10578680-44b5-4ffe-8754-7d5a4581db5d)

## **Features**
- Detects and extracts the chessboard from an image.
- Identifies the pieces on the board.
- Provides a structured representation of the board.

**Outputs statistics such as:**
- Number of pieces per team (White & Black).
- Position of each piece.
- Piece Notation

## The program uses the following notation to represent the chess pieces:

**X → Empty square**

**White Pieces (Uppercase):**
- C → Knight
- T → Rook
- Q → Queen
- B → Bishop
- K → King
- P → Pawn

**Black Pieces (Lowercase):**
- c → Knight
- t → Rook
- q → Queen
- b → Bishop
- k → King
- p → Pawn

## Demonstration
- [Youtube Video](https://youtu.be/g9NelrHHkfE)

## **How?**
This program is based on image processing and pixel manipulation.

Each pixel is divided into 3 layers:
- Red, which represents the reds
- Green, which represents the greens
- Blue, which represents the blues

You can manipulate the pixels to do certain things, such as
- Rotations
- Cropping
- Contrast
- Scale
- Convert to Black and White
- Mean
- Binarization
- Histograms

## Examples:
![Captura de ecrã 2025-03-15 230520](https://github.com/user-attachments/assets/8d1783db-5320-4f56-b508-209f52d52948)
![Captura de ecrã 2025-03-15 230508](https://github.com/user-attachments/assets/09303307-b046-40cf-905a-391343f11e0a)
![Captura de ecrã 2025-03-15 230655](https://github.com/user-attachments/assets/d78f0e79-c7ec-4592-babd-ddb56b1b3e06)
![Captura de ecrã 2025-03-15 230707](https://github.com/user-attachments/assets/71704a2f-fb24-45ad-b071-cec97235b418)
![Captura de ecrã 2025-03-15 230723](https://github.com/user-attachments/assets/8a5647ef-1359-4f3f-ad5e-c031c0028359)
![Captura de ecrã 2025-03-15 230645](https://github.com/user-attachments/assets/2de58c2c-d6b1-4157-8ea6-699a8108fb64)

