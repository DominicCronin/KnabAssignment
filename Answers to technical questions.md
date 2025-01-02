# Technical Questions

## 1. How long did you spend on the coding assignment? What would you add to your solution if you had more time? If you didn't spend much time on the coding assignment then use this as an opportunity to explain what you would add.

I did the exercise spread out over the holiday period. I think in total it cost me a few days. It was certainly more than the "couple of hours" that the recruiter had suggested it would be. This was partly because the exercise itself needs far more than a couple of hours, and partly because I chose myself to add some scope.

Specifically, because Aspire and React are both currently study areas for me, I chose to host my application in Aspire and to do the front end in React. Also, even though for an application of this scale, exceptions would have been fine, I wanted to get some practice at using the Result pattern. I hope you'll understand that my approach in these areas is necessarily less mature than in the rest.

What would I add if I had more time? Well to start with, a lot more logging and unit tests. I've added enough so that you can see that I know how to do these things, but this is not polished production code. My goals were to complete the assignment and along the way to learn something. I hope I've been able to achieve these things without having to add the hours it would have taken to complete these things.

One benefit of using Aspire is the opinionated defaults it comes with. In real life, I'd expect to go through the code in the ServiceDefaults project and make sure the various settings in here were aligned with the technical design/architecture. For example it's good to see that it comes with Resilience built-in. There are other concerns you might also look at, for example, maybe an output cache would make sense.

The front-end is pretty basic. For a real project there would need to be some attention paid to the style and usability, and more work on things like validation.

In general, though, a lot of the things that you might add would be driven by the requirements of the project. For a simple application like this, I'm satisfied that it works well enough, so YAGNI.

## 2. What was the most useful feature that was added to the latest version of your language of choice? Please include a snippet of code that shows how you've used it.

C# is a mature language, and although there were new features added in C#13, none of them were relevant to this project. I regularly use the primary constructors feature that was introduced in C#12. It helps to keep the code a little bit tidier, and that's always good. In the example below, you can see how the injected dependencies are processed without the need for an explicit constructor with boilerplate code assigning the various parameters to fields.

```
    public class ExchangeRatesApiClient(IOptions<ExchangeRatesApiOptions> options, ILogger<ExchangeRatesApiClient> logger, HttpClient client) : IExchangeRatesApiClient
    {
```

## 3. How would you track down a performance issue in production? Have you ever had to do this?
That depends on what kind of issue it is. Yes - I've had to do this, and every time it's different. I'd start with the symptoms. What is the problem that's been reported, and is there any evidence of this in the logs and metrics? Is the problem also reproducible in other environments? When did it begin? Can you correlate that with a code release or a change in the environment or configuration?
What is the impact? If an important system is failing, your approach is likely to be different than if it's something you can plan time for.

## 4. What was the latest technical book you have read or tech conference you have been to? What did you learn?
For what it's worth, I think the last technical book I read from beginning to end was "Mastering Regular Expressions" by Jeffrey Friedl. It's really interesting, and I learned a lot about the inner workings of regular expressions, the different dialects that exist and the features they each offer. That was a while ago now. These days I don't really read technical books. My sources of technical information and updating tend to be online. I have a fairly well-used [Pluralsight](https://app.pluralsight.com) account, where recently I've spent time updating on EF core and learning Go. As I mentioned above, I'm currently busy with React; I'm following [Josh Coumeau's course](https://courses.joshwcomeau.com/). I also follow various technical channels on YouTube, for example [Nick Chapsas](https://www.youtube.com/@nickchapsas). The most recent conference I attended was [Azure Lowlands](https://www.azurelowlands.com).

## 5. What do you think about this technical assessment?
First of all, I enjoy writing software, so I enjoyed it. I'd realised immediately that the scope was much larger than had been indicated. I couldn't justify the effort simply to support a job application, so I managed to make it also about my own learning. With such an assignment, you're very much in the hands of the people assessing it, so I hope to have the chance to discuss it with you.

## 6. Please, describe yourself using JSON.
```
{
    "name": "Dominic Cronin",
    "occupation": "Engineer",
    "nationality": ["British", "Dutch"],
    "livesIn": "Amsterdam",
    "family": [
        {   
            "relationship": "wife",
            "name": "Floor"
        },
        {   
            "relationship": "son",
            "name": "Joe"
        },
        {   
            "relationship": "son",
            "name": "Fin"
        },
        {   
            "relationship": "daughter",
            "name": "Kate"
        }
    ],
    "interests": {
        "sport": [
            "squash",
            "inline skating"
        ],
        "musical_instruments": [
            "harmonica",
            "tin whistle",
            "anglo concertina", 
            "piano"
        ]
    }
}
```