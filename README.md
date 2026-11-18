# HeyGen for Blackbird

Generate synthetic avatar videos from scripts or Studio templates. Use native HeyGen dubbing when you want the existing footage translated with voice cloning, or route editable scripts and template text through a TMS and a human review checkpoint before rendering a new video.

<!-- begin docs -->

## Connection

Create a HeyGen API key in the [API dashboard](https://platform.heygen.com/api-keys), then add a **HeyGen** connection in Blackbird. Use the key from the workspace that owns or can access your videos and other assets. The app sends the key as `x-api-key` to `https://api.heygen.com/v3`. It validates the connection using the current-key endpoint, which accepts any valid HeyGen API key without an extra account scope. Enable only the [permissions](https://developers.heygen.com/docs/api-key-permissions) needed by each Bird.

## Actions and triggers

- **Videos** — Search, get and download videos and captions, or create a single avatar video or multi-scene Studio video.
- **Templates and XLIFF** — Inspect template variables and render an API-ready template. Export video speech or template text as XLIFF for translation and human review. Read translated speech for Create video, or create a new template render directly from the approved file.
- **Audio** — Convert text or a recording into a Blackbird narration file, then render an avatar video or lip-sync footage with that file.
- **Native translation** — Translate video, Get video translation, Download translated video and Get translation captions. The proofreading actions let a linguist edit the translated SRT before the dubbed render.
- **Brand glossaries** — Search, get, create, update, delete and apply workspace rules.
- **Video Agent** — Generate drafts, list earlier cuts and revise selected scenes after review.
- **Lipsync and AI Clipping** — Submit jobs, read progress and retrieve results.
- **Batches** — Submit native video, translation, lipsync or asset batches. Read aggregate and item status.
- **Events** — On video completed, On video failed, On translation completed and On proofread ready are polling triggers. Workspace-wide polls establish a baseline. A proofread checkpoint scoped to one session also handles subtitles that are ready on the first check.

See [technical documentation](docs/technical-documentation.md) for scopes, action inputs and outputs, picker behavior, XLIFF constraints, and example Birds.

## API rate limits and concurrency

| Limit | Self-serve API | Enterprise API |
|---|---:|---:|
| General reads | 300/min | 1,000/min |
| General writes | 10/s | 30/s |
| Heavy operations such as asset upload | 30/min | 120/min |
| Job status | 500/min and 600/min per IP | 1,500/min and 1,800/min per IP |
| Native batch creation endpoints | 30/min | 30/min |
| Included concurrent workflows | 10 | 20 |
| Optional burst | — | Up to 50 extra per eligible workflow type at 1.5× contract rate |

Rate limits apply per user and endpoint unless indicated. HeyGen counts status paths with different resource IDs separately and also applies the IP cap. Enterprise burst is opt-in and applies to avatar renders and video translations. Individual jobs and batch items remain subject to the workspace entitlement. Check [HeyGen usage limits](https://developers.heygen.com/docs/usage-limits).

## API prices and credits

Provider-published self-serve examples, checked November 18, 2026. These are API rates, billed to the API balance. The credit figures are the values published by HeyGen alongside its USD rates. The account's API pricing page and contract govern actual charges.

| Operation | Self-serve USD per minute | Self-serve API credits per minute |
|---|---:|---:|
| Avatar III, Digital Twin | $0.60 | 12 |
| Avatar III, Studio or Photo Avatar | $0.99 | 20 |
| Avatar IV, Photo Avatar | $2.31 | 46 |
| Avatar IV, Digital Twin or Studio Avatar | $4.83 | 97 |
| Avatar V, Digital Twin | $7.20 | 144 |
| Video Agent | $5.82 | 116 |
| Translation, Speed audio-only | $0.57 | 11 |
| Translation, Speed lip sync | $0.81 | 16 |
| Translation, Precision lip sync | $1.50 | 30 |
| Lipsync, Speed | $0.84 | 17 |
| Lipsync, Precision | $1.74 | 35 |
| AI Clipping, per minute of source footage | $0.70 | 14 |

The same Help Center table lists Cinematic Avatar at **$7.30 / 146 credits per 720p render** or **$17.67 / 354 credits per 1080p render**. Cinematic Avatar is available through eligible native video batch payloads. HeyGen meters video and Agent output by actual seconds as described in its API pricing guidance. Verify the engine, avatar and media settings used by your template or batch before estimating costs. Public Enterprise schedules are unavailable. Use your contracted operation rates, with any opted-in burst multiplier, for Enterprise estimates. The documentation does not establish a separate public template or batch discount. Check the [API pricing guidance](https://help.heygen.com/en/articles/10060327-heygen-api-pricing-credits-explained) and the [account pricing page](https://platform.heygen.com/pricing).

A three-minute Precision translation into five languages submits five jobs and would produce about **900 seconds** of aggregate output, assuming each translation stays three minutes. At the published rate, that is **$22.50 and 450 credits**. Plan for output-length variation. Cap target languages and parallel flights, and use small batches when validating a new Bird. A batch acknowledges receipt before every item has succeeded.

## Review and revise Video Agent drafts

Use **List Video Agent drafts** and **List video scenes** to choose the cut and capture the exact edit version. **Revise selected scenes** accepts one instruction per selected scene and returns the new draft ID when available. Reuse ordinary video status and download actions for that ID. The [review guide](docs/review-to-localized-video-demo.md) shows how to approve the revised cut before sending it into translation proofreading. See [HeyGen scene edits](https://developers.heygen.com/docs/edit-scenes).

## Studio video composition

**Create Studio video** assembles 1–50 ordered scenes using a speaking avatar, still image or video clip. Use asset IDs, directly downloadable media URLs, or supported Blackbird files for media and narration. It returns a single video ID for the standard status and download actions. Choose **Automatic per Flight** for a runtime key, or supply a custom key for cross-Flight deduplication. In Automatic mode, assign different render slots to renders inside the same Flight. See [HeyGen Studio](https://developers.heygen.com/studio-videos) and the [CMS-to-localized-video blueprint](docs/CMS-to-localized-video-blueprint.md) for a release-note-to-localized-video example.

## Native translation proofreading

Start a proofread session for each target language. Wait with **On proofread ready** scoped to the session. Then download its translated SRT to Blackbird. Route it to a linguist through a Checkpoint or your TMS. After approval, upload the edited SRT to the same session and then generate the dubbed video. Map the returned **Translation ID** to **Get video translation**, **Download translated video** or **Get translation captions**. See the [HeyGen proofreading workflow](https://developers.heygen.com/docs/video-translate#proofread-before-finalizing).

Both Speed and Precision support proofreading on paid HeyGen API accounts. Session creation and final rendering use separate submission keys. Use the account pricing page for billing estimates, including any draft and render costs. See [HeyGen API pricing](https://help.heygen.com/en/articles/10060327-heygen-api-pricing-credits-explained).

## Audio production

**Text to speech** generates WAV or MP3 with the paid HeyGen Voice 1 model. **Speech to speech** uses an active instant voice in your workspace, preserves the recording's words and timing, and generates WAV. Both return a Blackbird file for a Checkpoint, **Create avatar video from audio**, or **Create lipsync**. The avatar action also accepts a direct HTTPS URL or a HeyGen audio asset ID. See the [TTS guide](https://developers.heygen.com/docs/voices/heygen-voice-speech) and [STS guide](https://developers.heygen.com/docs/voices/speech-to-speech).

HeyGen publishes a self-serve rate of **$15 / 300 credits per million characters** for instant voice speech on the standard model and **$0.12 / 2.4 credits per minute** for speech-to-speech. Check account terms for professional voices and contract rates. The documented speech limit is **30 requests per minute per endpoint**. The later avatar render is billed at its video generation rate. See the [API pricing guidance](https://help.heygen.com/en/articles/10060327-heygen-api-pricing-credits-explained) and [usage limits](https://developers.heygen.com/docs/usage-limits).

## Retrying jobs

For endpoints with HeyGen idempotency, provide a **Submission key** unique to the logical job. Map a durable value such as `asset-123:es:revision-7`. Reuse it only for the same job and inputs within HeyGen's 24-hour window. Use a new key for an intentionally new render. A correlation ID is separate.

The app retries HTTP 429 on reads only. It honors a short `Retry-After`, backs off when the header is absent, and surfaces longer waits. On an ambiguous write response, inspect HeyGen history and reuse the original submission key where applicable. Video Agent session creation and text-to-speech expose no documented idempotency key. Check session history or speech usage before manually trying again. Speech-to-speech requires the same key and identical inputs for a safe replay within 24 hours.

<!-- end docs -->

## Build and install

Run from the source root with .NET 8.

```sh
dotnet build Apps.HeyGen.sln -c Release
dotnet test Apps.HeyGen.sln -c Release
dotnet publish Apps.HeyGen/Apps.HeyGen.csproj -c Release -o publish
```

Zip the contents of `publish` at the archive root. In Blackbird open **Apps → My custom apps → Create app** and upload the deployment ZIP in step two. To update an existing installation of this package, open that app, choose **New version → Update App**, and upload a higher version. Keep Product and PackageId as `HeyGen` and AssemblyName as `Apps.HeyGen`. Keep saved action identities and existing field contracts stable for updates. See [Blackbird deployment](https://docs.blackbird.io/sdk/deploying/).

To run the online integration tests, copy `Tests.HeyGen/appsettings.json.example` to `appsettings.json` beside it, set the disposable resource IDs and URLs, and set the key in `HEYGEN_API_KEY` or the ignored file. Set `HEYGEN_TEST_ALLOW_WRITES=1` only in a disposable, funded workspace. Creation tests can consume credits. The glossary test cleans up only the glossary it creates.
