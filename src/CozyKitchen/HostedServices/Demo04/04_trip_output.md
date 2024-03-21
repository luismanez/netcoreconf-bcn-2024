How can I help:
I want to visit Rome for 3 days

Plan:

```json
{
  "Prompt": "<system~>## Instructions
Explain how to achieve the user's goal using the available helpers with a Handlebars .Net template.

## Example
If the user posed the goal below, you could answer with the following template.</system~>
<user~>## Goal
I want you to generate 10 random numbers and send them to another helper.</user~>
<assistant~>Here's a Handlebars template that achieves the goal:
```handlebars
{{!-- Step 0: Extract key values --}}
{{set
  \u0022count\u0022
  10
}}
{{!-- Step 1: Loop using the count --}}
{{#each
  (range
    1
    count
  )
}}
  {{!-- Step 2: Create random number --}}
  {{set
    \u0022randomNumber\u0022
    (Example-Random
      seed=this
    )
  }}
  {{!-- Step 3: Call example helper with random number and print the result to the screen --}}
  {{set
    \u0022result\u0022
    (Example-Helper
      input=randomNumber
    )
  }}
  {{json (concat \u0022The result\u0022 \u0022 \u0022 \u0022is:\u0022 \u0022 \u0022 result)}}
{{/each}}
```</assistant~>
<system~>Now let's try the real thing.</system~>
<user~>The following helpers are available to you:

## Built-in block helpers
- `{{#if}}{{/if}}`
- `{{#unless}}{{/unless}}`
- `{{#each}}{{/each}}`
- `{{#with}}{{/with}}`

## Loop helpers
If you need to loop through a list of values with `{{#each}}`, you can use the following helpers:
- `{{range}}` - Generates a list of integral numbers within a specified range, inclusive of the first and last value.
- `{{array}}` - Generates an array of values from the given values (zero-indexed).

IMPORTANT: `range` and `array` are the only supported data structures. Others like `hash` are not supported. Also, you cannot use any methods or properties on the built-in data structures.

## Math helpers
If you need to do basic operations, you can use these two helpers with numerical values:
- `{{add}}` - Adds two values together.
- `{{subtract}}` - Subtracts the second value from the first.

## Comparison helpers
If you need to compare two values, you can use the `{{equals}}` helper.
To use the math and comparison helpers, you must pass in two positional values. For example, to check if the variable `var` is equal to number `1`, you would use the following helper like so: `{{#if (equals var 1)}}{{/if}}`.

## Variable helpers
If you need to create or retrieve a variable, you can use the following helpers:
- `{{set}}` - Creates a variable with the given name and value. It does not print anything to the template, so you must use `{{json}}` to print the value.
- `{{json}}` - Generates and prints a JSON string from the given value.
- `{{concat}}` - Concatenates the given values into one string.

## Custom helpers
Lastly, you have the following custom helpers to use.

### `GraphSkillsPlugin-GetMySkills`
Description: Get current user's skills in their profile in MS Graph
Inputs:
Output: String

### `MyIpAddressPlugin-WhatsMyIp`
Description: Get your IP address
Inputs:
Output: String

### `ResumeAssistantPlugin-AboutMe`
Description: Creates an About Me paragraph that can be used in a Resume or Presentation.
Inputs:
    - FullName: String - User Full Name (required)
    - JobTitle: String - Your current Job title or the one that you want to use to generate the About me paragraph (i.e: Software Engineer, Lawyer) (required)
    - TotalYearsOfExperience: String - Number of years of experience (required)
    - MainArea: String - Main area or area of expertise of your Job Title. (required)
Output: string

### `ResumeAssistantPlugin-MySkillsDefinition`
Description: Creates a List of Skills returned by my Graph profile, with a brief definition for each skill
Inputs:
Output: string

### `TravelAgentPlugin-PlanMyTrip`
Description: Given a city and the number of days to spend in there, it gives you a detailed plan of what to do in that city on each day
Inputs:
    - City: String - The city you want to travel to and wanto to plan what to do in there (required)
    - NumberOfDays: String - The number of days you are going to be in the city. (required)
Output: string

### `UniversityFinderPlugin-ListByCountry`
Description: Get a list of universities in the given country
Inputs:
    - country: String - Country to find universities into (required)
    - top: Int32 - Number of universities to return (required)
Output: String

IMPORTANT: You can only use the helpers that are listed above. Do not use any other helpers that are not explicitly listed here. For example, do not use `{{log}}` or any `{{Example}}` helpers, as they are not supported.
</user~>
<user~>## Goal
I want to visit Rome for 3 days

</user~>

<system~>## Tips and reminders
- Add a comment above each step to describe what the step does.
- Each variable should have a well-defined name.
- Be extremely careful about types. For example, if you pass an array to a helper that expects a number, the template will error out.
- Each step should contain only one helper call.

## Start
Follow these steps to create one Handlebars template to achieve the goal:
0. Extract Key Values:
  - Read the goal and any user-provided content carefully and identify any relevant strings, numbers, or conditions that you'll need. Do not modify any data.
  - When generating variables or helper inputs, only use content that the user has explicitly provided or confirmed. If the user did not explicitly provide specific information, you should not invent or assume this information.
  - Use the `{{set}}` helper to create a variable for each key value.
  - Omit this step if no values are needed from the initial context.
1. Choose the Right Helpers:
  - Use the provided helpers to manipulate the variables you've created. Start with the basic helpers and only use custom helpers if necessary to accomplish the goal.
  - Always reference a custom helper by its full name.
2. Don't Create or Assume Unlisted Helpers:
  - Only use the helpers provided. Any helper not listed is considered hallucinated and must not be used.
  - Do not invent or assume the existence of any functions not explicitly defined above.
3. What if I Need More Helpers?
  - If the goal cannot be fully achieved with the provided helpers or you need a helper not defined, print the following message: \u0022Additional helpers or information may be required\u0022.
4. Keep It Simple:
  - Avoid using loops or block expressions. They are allowed but not always necessary, so try to find a solution that does not use them.
  - Your template should be intelligent and efficient, avoiding unnecessary complexity or redundant steps.
5. No Nested Helpers:
  - Do not nest helpers or conditionals inside other helpers. This can cause errors in the template.
6. Output the Result:
  - Once you have completed the necessary steps to reach the goal, use the `{{json}}` helper to output the final result.
  - Ensure your template and all steps are enclosed in a ``` handlebars block.

Remember, the objective is not to use all the helpers available, but to use the correct ones to achieve the desired outcome with a clear and concise template.
</system~>"
}

```

Plan results:

# Rome Trip Plan

## DAY 1
- **Morning:**
    - Start your day with a traditional Italian breakfast at **Roscioli Caffè Pasticceria**. Try their delicious pastries and coffee.
    - After breakfast, take a leisurely stroll around the **Campo de' Fiori Market**. This open-air market is a food lover's paradise with a variety of fresh produce, spices, and local delicacies.
- **Afternoon:**
    - For lunch, head to **Trattoria Da Enzo** in the Trastevere neighborhood. This place is known for its authentic Roman cuisine.
    - After lunch, join a **Roman Food Tour**. This will give you a chance to taste a variety of local dishes and learn about Rome's culinary history.
- **Evening:**
    - For dinner, visit **Pizzarium Bonci**, famous for its unique and delicious pizza.
    - End your day with a scoop of gelato from **Gelateria dei Gracchi**, one of the best gelato places in Rome.
## DAY 2
- **Morning:**
    - Start your day with breakfast at **Antico Forno Roscioli**, a historic bakery with a wide selection of bread and pastries.
    - Visit the **Testaccio Market**. It's less touristy than other Roman markets and offers a wide range of food stalls.
- **Afternoon:**
    - Have lunch at **Flavio al Velavevodetto**, a popular restaurant in Testaccio, known for its classic Roman dishes.
    - After lunch, take a **Cooking Class**. This is a fun way to learn about Roman cuisine and make some traditional dishes.
- **Evening:**
    - For dinner, try **Osteria Fernanda** for a more upscale dining experience. They offer a modern take on traditional Roman dishes.
    - Finish your day with a nightcap at **The Jerry Thomas Project**, a speakeasy-style bar known for its creative cocktails.
## DAY 3
- **Morning:**
    - Have breakfast at **Pasticceria De Bellis**, known for its innovative and delicious pastries.
    - Visit the **Prati District**. This neighborhood is known for its food shops and markets.
- **Afternoon:**
    - For lunch, try **Mordi e Vai** in the Prati Market. They are famous for their sandwiches made with traditional Roman ingredients.
    - After lunch, take a **Wine Tasting Tour**. Italy is famous for its wines, and this is a great way to learn about and taste a variety of Italian wines.
- **Evening:**
    - For your final dinner, visit **La Pergola**. This is Rome's only three-Michelin-starred restaurant and offers an unforgettable dining experience.
    - End your trip with a visit to **Bar del Fico** for a relaxed evening with a good selection of drinks and a lively atmosphere.