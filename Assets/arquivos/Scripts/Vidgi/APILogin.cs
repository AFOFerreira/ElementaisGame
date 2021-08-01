using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class APILogin
{
    public string code;
    public string message;
    public List<Data> data;
}
[Serializable]
 public class Data
{
    public string player_id;
    public string user_nick;
    public string user_email;
    public string user_name;
    public string user_last_name;
    public string tokens;
    public string player_team;
    public string user_img;
    public string user_tel;
    public string user_level;
    public string Id;
    public string event_game_code;
    public string event_daily;
    public string event_date;
    public string event_value;
    public string event_ticket;
    public string event_reward_mode;
    public string event_title;
    public string event_matrix_size;
    public string event_active;
    public string game_name;
    public string name;
}


