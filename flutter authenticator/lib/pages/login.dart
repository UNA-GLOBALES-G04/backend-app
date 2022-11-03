import 'package:another_flushbar/flushbar.dart';
import 'package:day_night_switcher/day_night_switcher.dart';
import 'package:flutter/material.dart';
import 'package:flutter_animated_dialog/flutter_animated_dialog.dart';
import 'package:oneauth/util/lang/language.dart';
import 'package:oneauth/util/lang_controller.dart';
import 'package:oneauth/util/theme_controller.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:wave/config.dart';
import 'package:wave/wave.dart';

/// Example app widget
class LoginPage extends StatelessWidget {
  /// Main app widget.
  const LoginPage({Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return const LoginScreen();
  }
}

/// Example login screen
class LoginScreen extends StatefulWidget {
  const LoginScreen({Key? key}) : super(key: key);

  @override
  State<LoginScreen> createState() => _LoginScreenState();
}

class _LoginScreenState extends State<LoginScreen> {
  bool _obscureText = true;
  bool _rememberMe = false;
  final urlController = TextEditingController();

  void _toggle() {
    setState(() {
      _obscureText = !_obscureText;
    });
  }

  void toggleRememberMe() {
    setState(() {
      _rememberMe = !_rememberMe;
    });
  }

  void toggleTheme() {
    ThemeController.of(context).toggleTheme();
  }

  bool setUrl(String url) {
    var uri = Uri.parse(url);
    if (uri.isAbsolute) {
      // save to shared preferences
      SharedPreferences.getInstance().then((prefs) {
        prefs.setString('url', url);
      });
      setState(() {
        urlController.text = url;
      });
      return true;
    }
    return false;
  }

  Future<String> url() async {
    var text = urlController.text;
    if (text.isEmpty) {
      var prefs = await SharedPreferences.getInstance();
      text = prefs.getString('url') ?? '';
    }
    return text;
  }

  @override
  Widget build(BuildContext context) {
    final lang = LanguageController.of(context);
    final isDarkMode = ThemeController.of(context).currentTheme == 'dark';
    LanguageController lc = LanguageController.of(context);

    return Scaffold(
      appBar: AppBar(
        leading: IconButton(
          icon: const Icon(Icons.settings),
          onPressed: () {
            // show a popup menu
            showAnimatedDialog(
                context: context,
                barrierDismissible: true,
                animationType: DialogTransitionType.slideFromTop,
                curve: Curves.fastOutSlowIn,
                duration: const Duration(milliseconds: 400),
                builder: (context) {
                  return AlertDialog(
                    title: Text(lc.getTranslation("settings-title")),
                    content: Column(
                      mainAxisSize: MainAxisSize.min,
                      children: [
                        TextField(
                          controller: urlController,
                          decoration: InputDecoration(
                            labelText: lc.getTranslation("settings-auth-url"),
                          ),
                        ),
                      ],
                    ),
                    actions: [
                      TextButton(
                        onPressed: () {
                          Navigator.of(context).pop();
                        },
                        child: Text(lc.getTranslation("cancel")),
                      ),
                      TextButton(
                        onPressed: () {
                          var url = urlController.text;
                          if (setUrl(url)) {
                            Navigator.of(context).pop();
                          } else {
                            Flushbar(
                              backgroundColor: Theme.of(context).errorColor,
                              message: lc.getTranslation("invalid-url"),
                              duration: const Duration(seconds: 3),
                            ).show(context);
                          }
                        },
                        child: Text(lc.getTranslation("save")),
                      ),
                    ],
                  );
                });
          },
        ),
        actions: [
          SizedBox(
            height: double.infinity,
            child: DayNightSwitcherIcon(
              isDarkModeEnabled: isDarkMode,
              // do nothing on state change
              onStateChanged: (_) {
                toggleTheme();
              },
            ),
          ),
          SizedBox(
            height: double.infinity,
            child: ElevatedButton(
              style: ElevatedButton.styleFrom(
                backgroundColor: Colors.transparent,
                elevation: 0,
              ),
              onPressed: () async {
                LanguageController lc = LanguageController.of(context);
                List<Language> languages = await lc.languages;

                // show a dialog to select the language
                showAnimatedDialog(
                  context: context,
                  barrierDismissible: true,
                  animationType: DialogTransitionType.slideFromRightFade,
                  curve: Curves.fastOutSlowIn,
                  duration: const Duration(milliseconds: 400),
                  builder: (context) => AlertDialog(
                    // popup animation
                    shape: RoundedRectangleBorder(
                      borderRadius: BorderRadius.circular(10),
                    ),
                    backgroundColor:
                        Theme.of(context).cardColor.withOpacity(0.8),
                    title: Text(lang.getTranslation('select-a-language')),
                    content: Column(
                      mainAxisSize: MainAxisSize.min,
                      // map its flag and name
                      // also add a default language
                      children:
                          // default system language
                          [
                                Padding(
                                  padding: const EdgeInsets.all(8.0),
                                  child: ListTile(
                                    title: Text(lc
                                        .getTranslation("system-default-lang")),
                                    onTap: () {
                                      lc
                                          .setPreferSystemLanguage(true)
                                          .then((_) {
                                        Navigator.pop(context);
                                        setState(() {});
                                      });
                                    },
                                  ),
                                )
                              ] +
                              languages
                                  .map(
                                    (e) => Padding(
                                      padding: const EdgeInsets.all(8.0),
                                      child: ListTile(
                                        leading: e.flag,
                                        title: Text(e.name),
                                        onTap: () {
                                          lc.setLanguage(e.code).then((c) => c
                                              .setPreferSystemLanguage(false)
                                              .then((_) => {
                                                    Navigator.pop(context),
                                                    setState(() {})
                                                  }));
                                        },
                                      ),
                                    ),
                                  )
                                  .toList(),
                    ),
                  ),
                );
              },
              child: FutureBuilder(
                future: lang.getFlag,
                builder: (context, snapshot) {
                  if (snapshot.hasData) {
                    return snapshot.data as Widget;
                  }
                  return const CircularProgressIndicator();
                },
              ),
            ),
          ),
        ],
      ),
      body: Stack(children: [
        // wave background
        Positioned(
          top: 0,
          left: 0,
          right: 0,
          child: SizedBox(
            // take all the screen
            height: MediaQuery.of(context).size.height,
            width: MediaQuery.of(context).size.width,
            child: WaveWidget(
              config: CustomConfig(
                // take system colors
                colors: [
                  const Color(0xFF114B5F).withOpacity(0.5),
                  const Color(0xFF456990).withOpacity(0.5),
                  const Color(0xFFE4FDE1).withOpacity(0.5),
                ],
                durations: [35000, 19440, 10800],
                heightPercentages: [0.20, 0.23, 0.25],
              ),
              size: const Size(double.infinity, double.infinity),
              waveAmplitude: 0,
            ),
          ),
        ),
        Center(
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            children: <Widget>[
              Container(
                  constraints: const BoxConstraints(maxWidth: 500),
                  child: createCard(context, lang)),
            ],
          ),
        ),
      ]),
    );
  }

  Card createCard(BuildContext context, LanguageController lang) {
    return Card(
      color: Theme.of(context).cardColor.withOpacity(0.8),
      elevation: 10,
      child: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: <Widget>[
            // the title
            Text(
              lang.getTranslation('login-title'),
              style: Theme.of(context).textTheme.headline6,
            ),
            // the form
            Form(
              child: Column(
                children: <Widget>[
                  // the email field
                  TextFormField(
                    decoration: InputDecoration(
                      labelText: lang.getTranslation('username-or-email'),
                    ),
                  ),
                  // the password field (with a button to show/hide the password)
                  TextFormField(
                    decoration: InputDecoration(
                      labelText: lang.getTranslation('password'),
                      suffixIcon: IconButton(
                        icon: const Icon(Icons.remove_red_eye),
                        onPressed: () {
                          _toggle();
                        },
                      ),
                    ),
                    obscureText: _obscureText,
                  ),
                  Flex(
                    // convert the direction to vertical if the width is less than 600
                    direction: Axis.horizontal,
                    children: <Widget>[
                      // remember me checkbox
                      Row(
                        children: <Widget>[
                          Checkbox(
                            value: _rememberMe,
                            onChanged: (bool? value) {
                              toggleRememberMe();
                            },
                          ),
                          Text(lang.getTranslation('remember-me')),
                        ],
                      ),
                      // forgot password text
                      const Spacer(),
                      TextButton(
                        onPressed: () {},
                        child: Text(
                          lang.getTranslation('forgot-password'),
                        ),
                      ),
                    ],
                  ),

                  Padding(
                    padding: const EdgeInsets.symmetric(vertical: 16.0),
                    child: SizedBox(
                      width: double.infinity,
                      child: ElevatedButton(
                        onPressed: () {
                          // do nothing
                        },
                        child: Text(lang.getTranslation('login')),
                      ),
                    ),
                  ),
                  // register text
                  SizedBox(
                    width: double.infinity,
                    child: TextButton(
                      onPressed: () {},
                      child: Text(
                        lang.getTranslation('register'),
                      ),
                    ),
                  ),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }
}
