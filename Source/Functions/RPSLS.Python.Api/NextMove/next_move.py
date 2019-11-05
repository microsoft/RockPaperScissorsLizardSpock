import logging
import random
import os
import json
from typing import Tuple, List

import requests


def predict(player_name: str) -> str:
    next_move = _predict_next_move(*_get_player_games(player_name))
    return _convert_game_to_json(next_move)


R_rock, P_paper, S_scissors, V_spock, L_lizard = ('R', 'P', 'S', 'V', 'L')
INTERNAL_MOVES_ENCODING = [R_rock, P_paper, S_scissors, V_spock, L_lizard]


def _get_player_games(player_name: str) -> Tuple[str, str]:
    game_manager_uri = os.getenv("GAME_MANAGER_URI", None)
    url = f'{game_manager_uri}/game-manager/api/games?player={player_name}'

    logging.info(f'requesting human moves: {url}')
    req = requests.get(url)
    data = req.json()
    return _convert_games_to_str(data["challengerGames"]), _convert_games_to_str(data["humanGames"])


def _convert_games_to_str(games) -> str:
    SOURCE_MOVES_ENCODING = [R_rock, P_paper, S_scissors, L_lizard, V_spock]
    return "".join([SOURCE_MOVES_ENCODING[game] for game in games])


def _convert_game_to_json(game: str) -> str:
    JSON_MOVES_ENCODING = {R_rock: "rock", P_paper: "paper",
                           S_scissors: "scissors", L_lizard: "lizard", V_spock: "spock"}
    return json.dumps({"prediction": JSON_MOVES_ENCODING[game]})


def _zip_moves(challenger_moves: List[str], human_moves: List[str]) -> List[Tuple[str, str]]:
    move_encoding_dict = {value: index for index, value in enumerate(INTERNAL_MOVES_ENCODING)}
    history = [(move_encoding_dict[i], move_encoding_dict[j])
               for i, j in zip(challenger_moves, human_moves)]
    return history


def _predict_next_move(challenger_moves: str, human_moves: str) -> str:
    history = _zip_moves(challenger_moves, human_moves)

    # what would have been predicted in the last rounds?
    pred_hist = [_best_next_moves_for_game(
        history[:i]) for i in range(2, len(history)+1)]

    # if no history prediction, then returns random
    if not pred_hist:
        return random.choice(INTERNAL_MOVES_ENCODING)

    # how would the different predictions have scored?
    # we have the pred_hist from moves i=2 to len(history) so we can check
    # check https://i.stack.imgur.com/jILea.png for game rules
    n_pred = len(pred_hist[0])
    scores = [[0]*5 for i in range(n_pred)]
    for pred, real in zip(pred_hist[:-1], history[2:]):
        for i in range(n_pred):
            # %5: When an int is negative it returns the count to the move
            # to beat another, in (reverse order) counterclockwise
            # i.e -1%5=4, -2%5=3
            scores[i][(real[1]-pred[i]+1) % 5] += 1
            scores[i][(real[1]-pred[i]+3) % 5] += 1
            # 1 & 3 move to the other "moves" that beat another
            # for example Rock is beaten with Paper and Spock,
            # which are 1 & 3 positions away
            scores[i][(real[1]-pred[i]+2) % 5] -= 1
            scores[i][(real[1]-pred[i]+4) % 5] -= 1

    # depending in predicted strategies, select best one with less risks
    # return best counter move
    best_scores = [list(max(enumerate(s), key=lambda x: x[1])) for s in scores]
    best_scores[-1][1] *= 1.001   # bias towards the simplest strategy
    if best_scores[-1][1] < 0.4*len(history):
        best_scores[-1][1] *= 1.4
    strat, (shift, _) = max(enumerate(best_scores), key=lambda x: x[1][1])

    return INTERNAL_MOVES_ENCODING[(pred_hist[-1][strat]+shift) % 5]


def _best_next_moves_for_game(hist: List[str]) -> List[List[str]]:

    N = len(hist)
    # find longest match of the preceding moves in the earlier history
    cand_m = cand_o = cand_b = range(N-1)

    for l in range(1, min(N, 20)):
        ref = hist[N-l]

        # l = 1
        # Looks for previous occurrences of the last move in my_moves, since hist[N-l] == hist[-1]
        # l = 2
        # it checks which of the possible candidates was preceded by the move previous to the last
        # and so on... i.e loos for longest chain matching last moves to use the next move
        cand_m_tmp = []
        for c in cand_m:
            if c >= l and hist[c-l+1][0] == ref[0]:
                cand_m_tmp.append(c)
        if not cand_m_tmp:
            cand_m = cand_m[-1:]
        else:
            cand_m = cand_m_tmp[:]

        # same for op_moves
        cand_o_tmp = []
        for c in cand_o:
            if c >= l and hist[c-l+1][1] == ref[1]:
                cand_o_tmp.append(c)
        if not cand_o_tmp:
            cand_o = cand_o[-1:]
        else:
            cand_o = cand_o_tmp[:]

        # same for both_moves i.e directly the zipped tuples
        cand_b_tmp = []
        for c in cand_b:
            if c >= l and hist[c-l+1] == ref:
                cand_b_tmp.append(c)
        if not cand_b_tmp:
            cand_b = cand_b[-1:]
        else:
            cand_b = cand_b_tmp[:]

    # analyze which moves were used how often, i.e a np.bincount
    freq_m, freq_o = [0]*5, [0]*5
    for m in hist:
        freq_m[m[0]] += 1
        freq_o[m[1]] += 1

    # return predictions (or possible "good" strategies)
    last_2_moves = [j for i in hist[:-3:-1] for j in i]
    return (last_2_moves +   # repeat last moves
            [hist[cand_m[-1]+1][0],     # history matching of my own moves
                # history matching of opponent's moves
                hist[cand_o[-1]+1][1],
                hist[cand_b[-1]+1][0],     # history matching of both
                hist[cand_b[-1]+1][1],
                freq_m.index(max(freq_m)),  # my most frequent move
                freq_o.index(max(freq_o)),  # opponent's most frequent move
                0])
