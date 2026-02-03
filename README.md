# Unity Poker Game

A fully-functional Texas Hold'em poker game built in Unity with bot opponents, proper hand evaluation, side pot support, and turn-based gameplay.

## 🎮 Features

### Core Gameplay
- **Texas Hold'em Rules**: Complete implementation of standard Texas Hold'em poker
- **4-Player Support**: Play against 3 bot opponents
- **Full Betting System**: Check, bet, call, raise, fold, and all-in support
- **Side Pot Management**: Automatic side pot calculation when multiple players go all-in with different amounts
- **Street Progression**: Pre-flop, flop, turn, river, and showdown phases

### Hand Evaluation
- **Accurate Hand Rankings**: Detects all poker hands from High Card to Royal Flush
- **Tie-Breaking System**: Properly compares hands of the same rank using kickers
- **Best Hand Selection**: Automatically finds the best 5-card combination from 7 cards (2 hole + 5 community)

### Bot Opponents
- **Bot Strategy System**: Different strategies (smart, aggressive, conservative etc)
- **Monte Carlo Simulation**: Estimates hand strength using simulation
- **Dynamic Betting**: Bots adjust their play based on hand strength and pot odds

### Visual Features
- **Card Display**: Visual representation of player hands and community cards
- **Chip Stacks**: Real-time stack size display for all players
- **Pot Tracking**: Live pot amount updates
- **Turn Indicators**: Clear indication of whose turn it is
- **Dealer Button**: Rotates properly between hands

## 📋 Requirements

- Unity 2022.3 or later
- C# 9.0+
- No external dependencies required

## 🎯 How to Play

### Starting a Hand

1. The game automatically starts with 4 players, each with 1,000 chips
2. Small blind (10 chips) and big blind (20 chips) are posted automatically
3. Each player receives 2 hole cards

### Betting Actions

- **Check**: Pass the action without betting (only when no bet to call)
- **Bet**: Make the first bet in a round
- **Call**: Match the current bet
- **Raise**: Increase the current bet (minimum raise: 20 chips)
- **Fold**: Discard your hand and forfeit the pot
- **All-In**: Bet all remaining chips

### Game Flow

1. **Pre-flop**: Players act based on their 2 hole cards
2. **Flop**: 3 community cards revealed, betting round
3. **Turn**: 1 more community card revealed, betting round
4. **River**: Final community card revealed, last betting round
5. **Showdown**: Remaining players reveal hands, best hand(s) win

## 🏆 Hand Rankings

From highest to lowest:

1. **Royal Flush**: A♠ K♠ Q♠ J♠ 10♠
2. **Straight Flush**: 9♥ 8♥ 7♥ 6♥ 5♥
3. **Four of a Kind**: K♠ K♥ K♦ K♣ A♠
4. **Full House**: Q♠ Q♥ Q♦ 3♠ 3♥
5. **Flush**: A♦ J♦ 9♦ 5♦ 3♦
6. **Straight**: 9♠ 8♥ 7♦ 6♣ 5♠
7. **Three of a Kind**: 7♠ 7♥ 7♦ A♠ K♥
8. **Two Pair**: J♠ J♥ 8♦ 8♣ A♠
9. **Pair**: 10♠ 10♥ K♦ Q♣ 9♠
10. **High Card**: A♠ K♥ Q♦ 8♣ 5♠

## 🐛 Known Issues & Solutions

### Game Freezes After Large Bet
**Problem**: Game enters infinite loop when a player bets all chips

**Solution**: Updated `IsBettingRoundComplete()` and `NextPlayer()` methods to properly handle all-in situations with safety counters.

### All-In Bets Not Showing
**Problem**: When bots go all-in, the UI doesn't update

**Solution**: Added `UpdateAllPlayerUI()` immediately after each action in `Call()`, `Raise()`, `Bet()`, `Fold()`, and `Check()` methods.

### Incorrect Winner in Ties
**Problem**: Wrong player wins when both have same hand rank

**Solution**: Updated `HandEvaluator.BestHandValue()` to properly handle hand strength comparison.


## 🎨 Future Customization Ideas...

- **Add animations**: Card dealing, chip movements, player reactions
- **Sound effects**: Chip sounds, card shuffle, dealer voice
- **Multiple table themes**: Different felt colors and card designs
- **Statistics tracking**: Track wins, hands played, biggest pot
- **Hand history**: Record and replay previous hands

---

**Enjoy the game! Good luck at the tables! 🃏**
