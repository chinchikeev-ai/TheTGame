public enum PatronCommentaryEvent
{
    PatronSelected, WaveStarted, WaveClosing, WaveCompleted, DefenseBuilt, DefenseSold,
    GateDamaged, EnemyLeaked, KillMilestone, BossEntered, BossDefeated, HectorDowned,
    HectorRevived, DivinePowerUsed
}

public static class DivinePatronCommentaryCatalog
{
    public static string PatronName(DivineGiftType patron)
    {
        switch (patron)
        {
            case DivineGiftType.Ares: return GameLanguage.T("ARES • GOD OF WAR", "АРЕС • БОГ ВОЙНЫ");
            case DivineGiftType.Athena: return GameLanguage.T("ATHENA • GODDESS OF TACTICS", "АФИНА • БОГИНЯ ТАКТИКИ");
            case DivineGiftType.Apollo: return GameLanguage.T("APOLLO • GOD OF LIGHT", "АПОЛЛОН • БОГ СВЕТА");
            default: return GameLanguage.T("POSEIDON • GOD OF THE SEA", "ПОСЕЙДОН • БОГ МОРЯ");
        }
    }

    public static string Line(DivineGiftType patron, PatronCommentaryEvent eventType, int value = 0)
    {
        switch (eventType)
        {
            case PatronCommentaryEvent.PatronSelected:
                return Voice(patron,
                    "Good. Give me a battle worth watching!", "Хорошо. Покажите мне битву, достойную внимания!",
                    "I am watching. Make every position count.", "Я наблюдаю. Пусть каждая позиция будет выбрана не зря.",
                    "My light is upon Troy. Let us begin.", "Мой свет над Троей. Начнём.",
                    "The sea brought them. I will watch it take them back.", "Море принесло их. Посмотрим, как оно заберёт их обратно.");
            case PatronCommentaryEvent.WaveStarted:
                return Voice(patron,
                    $"Wave {value}! Now we find out who came to fight.", $"Волна {value}! Сейчас узнаем, кто пришёл сражаться.",
                    $"Wave {value}. Hold the route and preserve the reserve.", $"Волна {value}. Держите путь и берегите резерв.",
                    $"Wave {value} enters the light. Loose your arrows.", $"Волна {value} вышла на свет. Спускайте тетивы.",
                    $"Wave {value}. Let them struggle against the current.", $"Волна {value}. Пусть попробуют пойти против течения.");
            case PatronCommentaryEvent.WaveClosing:
                return Voice(patron,
                    "They are wavering. Finish this loudly!", "Они дрогнули. Закончите громко!",
                    "Their formation is broken. Close the trap.", "Их строй разрушен. Закрывайте ловушку.",
                    "Only a few remain in the light.", "На свету осталось лишь несколько врагов.",
                    "The last of them are caught in the undertow.", "Последних уже тянет обратным течением.");
            case PatronCommentaryEvent.WaveCompleted:
                return Voice(patron,
                    "Ha! They will need a larger army.", "Ха! Им понадобится армия побольше.",
                    "Successful defense. Rebuild before the next move.", "Оборона успешна. Перестройтесь перед следующим ходом.",
                    "The field is clear again.", "Поле снова очищено.",
                    "One wave recedes. Another always follows.", "Одна волна отступила. За ней всегда приходит следующая.");
            case PatronCommentaryEvent.DefenseBuilt:
                return Voice(patron,
                    "A new warrior joins the argument!", "Новый воин добавил свой довод!",
                    "Useful placement. Now protect its blind side.", "Полезная позиция. Теперь прикройте её слабую сторону.",
                    "A fine position beneath the sun.", "Хорошая позиция под солнцем.",
                    "A stone against the tide. Let us see if it holds.", "Камень против прилива. Посмотрим, устоит ли он.");
            case PatronCommentaryEvent.DefenseSold:
                return Voice(patron,
                    "Changing weapons mid-battle? Bold.", "Меняете оружие посреди боя? Смело.",
                    "Resources recovered. Spend them with purpose.", "Ресурсы возвращены. Потратьте их осмысленно.",
                    "One light fades so another may burn brighter.", "Один свет гаснет, чтобы другой горел ярче.",
                    "Even fortifications must move with the tide.", "Даже укрепления должны следовать приливу.");
            case PatronCommentaryEvent.GateDamaged:
                return Voice(patron,
                    $"The gate lost {value}! Hit them back twice as hard.", $"Ворота потеряли {value}! Ответьте им вдвое сильнее.",
                    $"Gate damage: {value}. Correct the breach immediately.", $"Урон воротам: {value}. Немедленно закройте брешь.",
                    $"Troy's gate is wounded by {value}. Do not let the light fail.", $"Ворота Трои потеряли {value}. Не дайте свету погаснуть.",
                    $"The tide reached the gate for {value}. Push it back.", $"Прилив достал до ворот на {value}. Отбросьте его.");
            case PatronCommentaryEvent.EnemyLeaked:
                return Voice(patron,
                    "One slipped through. I expect an answer.", "Один прорвался. Я жду ответа.",
                    "A leak reveals the weak point. Fix it.", "Прорыв показал слабое место. Исправьте его.",
                    "A shadow passed the line. Do not allow another.", "Тень прошла сквозь строй. Не пропустите следующую.",
                    "One escaped the current. The next will not.", "Один вырвался из течения. Следующий не сможет.");
            case PatronCommentaryEvent.KillMilestone:
                return Voice(patron,
                    $"{value} enemies down! Now this is entertainment.", $"Уже {value} врагов! Вот теперь интересно.",
                    $"{value} defeated. Their pressure is predictable.", $"Повержено {value}. Их натиск становится предсказуемым.",
                    $"{value} have fallen beneath Troy's light.", $"{value} пали под светом Трои.",
                    $"The sea counts {value} debts repaid.", $"Море насчитало {value} возвращённых долгов.");
            case PatronCommentaryEvent.BossEntered:
                return Voice(patron,
                    "At last, someone important enough to hit!", "Наконец-то появился тот, кого стоит ударить!",
                    "Menelaus enters. Isolate his support.", "Вышел Менелай. Отсеките его поддержку.",
                    "Menelaus stands revealed. Mark him well.", "Менелай вышел на свет. Отметьте его как следует.",
                    "A king steps into deep water.", "Царь зашёл слишком глубоко в воду.");
            case PatronCommentaryEvent.BossDefeated:
                return Voice(patron,
                    "A king falls like any other soldier!", "Царь падает так же, как любой солдат!",
                    "The commander is down. The objective is secured.", "Командир пал. Цель удержана.",
                    "Menelaus has left the light.", "Менелай покинул свет.",
                    "The sea will remember that fall.", "Море запомнит это падение.");
            case PatronCommentaryEvent.HectorDowned:
                return Voice(patron,
                    "Hector! Get up — the battle is not finished!", "Гектор! Вставай — битва ещё не закончена!",
                    "Hector is down. Stabilize the line.", "Гектор пал. Удерживайте строй.",
                    "The prince's light dims, but it is not gone.", "Свет царевича померк, но не погас.",
                    "Even heroes sink. Strong ones rise again.", "Даже герои уходят под воду. Сильные поднимаются снова.");
            case PatronCommentaryEvent.HectorRevived:
                return Voice(patron,
                    "There he is! Back into the fight, Hector!", "Вот он! Снова в бой, Гектор!",
                    "Hector has returned. Re-form around him.", "Гектор вернулся. Перестройте строй вокруг него.",
                    "The prince shines again.", "Царевич снова сияет.",
                    "Hector rises with the next wave.", "Гектор поднялся вместе с новой волной.");
            default:
                return Voice(patron,
                    "Divine power! Make the moment count.", "Божественная сила! Используйте этот миг.",
                    "Power committed. Exploit the opening.", "Сила применена. Используйте преимущество.",
                    "The battlefield answers the divine light.", "Поле боя отвечает божественному свету.",
                    "The battlefield moves like water.", "Поле боя движется, словно вода.");
        }
    }

    static string Voice(DivineGiftType patron,
        string aresEn, string aresRu, string athenaEn, string athenaRu,
        string apolloEn, string apolloRu, string poseidonEn, string poseidonRu)
    {
        switch (patron)
        {
            case DivineGiftType.Ares: return GameLanguage.T(aresEn, aresRu);
            case DivineGiftType.Athena: return GameLanguage.T(athenaEn, athenaRu);
            case DivineGiftType.Apollo: return GameLanguage.T(apolloEn, apolloRu);
            default: return GameLanguage.T(poseidonEn, poseidonRu);
        }
    }
}
